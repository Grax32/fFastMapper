using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using System.ComponentModel;
using System.Threading;
using System.Linq.Expressions;
using System.Diagnostics;

namespace Grax.fFastImplementer
{
    public class fFastImplementInterface
    {


        public class fFastSelfNotifyingBase : INotifyPropertyChanging, INotifyPropertyChanged
        {
            /// <summary>
            /// Method to be called prior to property being set.
            /// </summary>
            /// <typeparam name="T">Type of property</typeparam>
            /// <param name="propertyName">Name of property</param>
            /// <param name="newValue">New value for property</param>
            /// <returns>true to abort, false to continue</returns>
            public virtual bool OnValueSetting<T>(string propertyName, T newValue)
            {
                RaisePropertyChangingEvent(propertyName);
                return fFastImplementInterfaceConstants.ContinuePropertySave;
            }

            /// <summary>
            /// Method to be called after property has been set.
            /// </summary>
            /// <param name="propertyName">Name of property</param>
            /// <param name="newValue">New value for property</param>
            public virtual void OnValueSet<T>(string propertyName, T newValue)
            {
                RaisePropertyChangedEvent(propertyName);
            }

            protected void RaisePropertyChangingEvent(string propertyName)
            {
                if (PropertyChanging != null)
                {
                    PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
                }
            }

            protected void RaisePropertyChangedEvent(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            public event PropertyChangingEventHandler PropertyChanging;
            public event PropertyChangedEventHandler PropertyChanged;
        }

        private static Type fFastSelfNotifyingBaseType = typeof(fFastSelfNotifyingBase);

        private static class InterfaceConstructor<TInterface, TBaseType>
            where TInterface : class
            where TBaseType : class
        {
            /// <summary>
            /// Runs once (once per TInterface,TBaseType type) when this type is first used
            /// </summary>
            internal static Func<TInterface> CreateNew = GetInterfaceConstructor();
            internal static Type ImplementerType { get; private set; }

            private static Func<TInterface> GetInterfaceConstructor()
            {
                Debug.WriteLine("Constructing Type for " + typeof(TInterface).Name);
                ImplementerType = CreateInterfaceImplementer(typeof(TInterface), typeof(TBaseType));
                var constructor = ImplementerType.GetConstructor(Type.EmptyTypes);
                return (Func<TInterface>)Expression.Lambda(Expression.New(constructor)).Compile();
            }
        }

        /// <summary>
        /// Create a new instance of interface specifying a custom base class
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TBaseType"></typeparam>
        /// <returns></returns>
        public static TInterface CreateNewWithBase<TInterface, TBaseType>()
            where TInterface : class
            where TBaseType : class
        {
            return InterfaceConstructor<TInterface, TBaseType>.CreateNew();
        }

        /// <summary>
        /// Create a new instance of interface using standard base class
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns></returns>
        public static TInterface CreateNew<TInterface>()
            where TInterface : class
        {
            return InterfaceConstructor<TInterface, fFastSelfNotifyingBase>.CreateNew();
        }

        public static Type CreateInterfaceImplementer(Type interfaceType)
        {
            return CreateInterfaceImplementer(interfaceType, null);
        }

        public static Type CreateInterfaceImplementer(Type interfaceType, Type baseType)
        {
            baseType = baseType ?? fFastSelfNotifyingBaseType;

            var typeName = interfaceType.Name + "Implementer";
            var asmName = new AssemblyName { Name = typeName + "DynamicAssembly" };

            AssemblyBuilder newAssembly;

            var typeBuilder = (newAssembly = Thread.GetDomain().DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run))
                .DefineDynamicModule(typeName + "Module")
                .DefineType(typeName, TypeAttributes.Class | TypeAttributes.Public, baseType, new Type[] { interfaceType });

            foreach (var property in interfaceType.GetProperties())
            {
                CreateProperty(typeBuilder, interfaceType, baseType, property.Name, property.PropertyType);
            }

            if (interfaceType.GetMethods().Any(v => !v.IsSpecialName) ||
                interfaceType.GetEvents().Any() ||
                interfaceType.GetCustomAttributes(true).OfType<DefaultMemberAttribute>().Any()
                )
            {
                throw new Exception(@"fFastImplementer supports auto-implementation of interfaces with only properties.
There is no support for Methods, Events, or Indexers");
            }

            return typeBuilder.CreateType();
        }

        private static MethodAttributes _GetterSetterAttributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

        private static void CreateProperty(TypeBuilder typeBuilder, Type interfaceType, Type baseType, string propertyName, Type propertyType)
        {
            var createdProperty = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            var newPropField = typeBuilder.DefineField(
                "_" + propertyName,
                propertyType,
                FieldAttributes.Private);

            // Define the "get" accessor method
            var createdPropertyGetAccessor = typeBuilder.DefineMethod(
                "get_" + propertyName,
                _GetterSetterAttributes,
                propertyType,
                Type.EmptyTypes);

            ILGenerator createdPropertyGetterIL = createdPropertyGetAccessor.GetILGenerator();
            createdPropertyGetterIL.Emit(OpCodes.Ldarg_0);
            createdPropertyGetterIL.Emit(OpCodes.Ldfld, newPropField);
            createdPropertyGetterIL.Emit(OpCodes.Ret);

            // Define the "set" accessor method 
            var createdPropertySetAccessor = typeBuilder.DefineMethod(
                "set_" + propertyName,
                _GetterSetterAttributes,
                null,
                new Type[] { propertyType });

            ILGenerator createdPropertySetterIL = createdPropertySetAccessor.GetILGenerator();

            var exitMethodLabel = createdPropertySetterIL.DefineLabel();

            var onValueSettingMethod = GetOnValueSettingMethod(baseType);

            if (onValueSettingMethod != null)
            {
                createdPropertySetterIL.Emit(OpCodes.Ldarg_0);

                foreach (var parm in onValueSettingMethod.GetParameters())
                {
                    if (parm.ParameterType == typeof(string))
                    {
                        createdPropertySetterIL.Emit(OpCodes.Ldstr, propertyName);
                    }

                    if (parm.ParameterType.IsGenericParameter)
                    {
                        createdPropertySetterIL.Emit(OpCodes.Ldarg_1);
                    }
                }

                if (onValueSettingMethod.ContainsGenericParameters)
                {
                    onValueSettingMethod = onValueSettingMethod.MakeGenericMethod(propertyType);
                }

                createdPropertySetterIL.Emit(OpCodes.Call, onValueSettingMethod);

                if (onValueSettingMethod.ReturnType == typeof(bool))
                {
                    createdPropertySetterIL.Emit(OpCodes.Brtrue, exitMethodLabel);
                }
            }


            createdPropertySetterIL.Emit(OpCodes.Ldarg_0);
            createdPropertySetterIL.Emit(OpCodes.Ldarg_1);
            createdPropertySetterIL.Emit(OpCodes.Stfld, newPropField);

            var onValueSetMethod = GetOnValueSetMethod(baseType);

            if (onValueSetMethod != null)
            {
                createdPropertySetterIL.Emit(OpCodes.Ldarg_0);

                foreach (var parm in onValueSetMethod.GetParameters())
                {
                    if (parm.ParameterType == typeof(string))
                    {
                        createdPropertySetterIL.Emit(OpCodes.Ldstr, propertyName);
                    }

                    if (parm.ParameterType.IsGenericParameter)
                    {
                        createdPropertySetterIL.Emit(OpCodes.Ldarg_1);
                    }
                }

                if (onValueSetMethod.ContainsGenericParameters)
                {
                    onValueSetMethod = onValueSetMethod.MakeGenericMethod(propertyType);
                }

                createdPropertySetterIL.Emit(OpCodes.Call, onValueSetMethod);
            }

            createdPropertySetterIL.MarkLabel(exitMethodLabel);
            createdPropertySetterIL.Emit(OpCodes.Ret);

            //newPropSetIL.Emit(OpCodes.Ldarg_0);
            //newPropSetIL.Emit(OpCodes.Ldarg_1);
            //newPropSetIL.Emit(OpCodes.Stfld, newPropField);
            //newPropSetIL.Emit(OpCodes.Ret);

            // Map the accessor methods
            createdProperty.SetGetMethod(createdPropertyGetAccessor);
            createdProperty.SetSetMethod(createdPropertySetAccessor);

            if (interfaceType != null)
            {
                var interfaceProperty = interfaceType.GetProperty(propertyName);
                if (interfaceProperty != null)
                {
                    typeBuilder.DefineMethodOverride(createdPropertySetAccessor, interfaceProperty.GetSetMethod());
                    typeBuilder.DefineMethodOverride(createdPropertyGetAccessor, interfaceProperty.GetGetMethod());
                }
            }
        }

        private static MethodInfo GetOnValueSettingMethod(Type baseType)
        {
            var onValueSettingMethods = baseType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(v => v.Name == "OnValueSetting");

            if (onValueSettingMethods.Count() > 0)
            {
                // string = property name
                // generic = propery value
                var methodsWith2Parms = onValueSettingMethods.Where(v =>
                {
                    var parms = v.GetParameters();

                    return parms.Length == 2 &&
                        parms.Any(parm => parm.ParameterType == typeof(string)) &&
                        parms.Any(parm => parm.ParameterType.IsGenericParameter);
                });

                if (methodsWith2Parms.Count() > 0)
                {
                    var onValueSettingMethod =
                        methodsWith2Parms.FirstOrDefault(v => v.ReturnType == typeof(bool)) ??
                        methodsWith2Parms.FirstOrDefault(v => v.ReturnType == typeof(void));

                    if (onValueSettingMethod != null)
                    {
                        return onValueSettingMethod;
                    }
                }

                // string = property name
                // generic = propery value
                var methodsWith1StringParm = onValueSettingMethods.Where(v =>
                {
                    var parms = v.GetParameters();

                    return parms.Length == 1 &&
                        parms.Any(parm => parm.ParameterType == typeof(string));
                });

                if (methodsWith1StringParm.Count() > 0)
                {
                    return
                        methodsWith1StringParm.FirstOrDefault(v => v.ReturnType == typeof(bool)) ??
                        methodsWith1StringParm.FirstOrDefault(v => v.ReturnType == typeof(void));
                }
            }
            return null;
        }

        private static MethodInfo GetOnValueSetMethod(Type baseType)
        {
            var onValueSetMethods = baseType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(v => v.Name == "OnValueSet");

            if (onValueSetMethods.Count() > 0)
            {
                // string = property name
                // generic = propery value
                var methodsWith2Parms = onValueSetMethods.Where(v =>
                {
                    var parms = v.GetParameters();

                    return parms.Length == 2 &&
                        parms.Any(parm => parm.ParameterType == typeof(string)) &&
                        parms.Any(parm => parm.ParameterType.IsGenericParameter);
                });

                if (methodsWith2Parms.Count() > 0)
                {
                    var onValueSettingMethod =
                        methodsWith2Parms.FirstOrDefault(v => v.ReturnType == typeof(void));

                    if (onValueSettingMethod != null)
                    {
                        return onValueSettingMethod;
                    }
                }

                // string = property name
                // generic = propery value
                var methodsWith1StringParm = onValueSetMethods.Where(v =>
                {
                    var parms = v.GetParameters();

                    return parms.Length == 1 &&
                        parms.Any(parm => parm.ParameterType == typeof(string));
                });

                if (methodsWith1StringParm.Count() > 0)
                {
                    return
                        methodsWith1StringParm.FirstOrDefault(v => v.ReturnType == typeof(void));
                }
            }
            return null;
        }
    }
}