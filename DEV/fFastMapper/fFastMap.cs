using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Grax.fFastMapper
{
    /// <summary>
    /// A mapping component that allows the developer to set up high-performing mappings between data types
    /// </summary>
    public static class fFastMap
    {
        static fFastMap()
        {
            AutoInitialize = true;
        }

        private static fFastMapperGlobalSettingsFluent _globalSettings = new fFastMapperGlobalSettingsFluent();
        private static MappingDirection _defaultMappingDirection = MappingDirection.Bidirectional;
        internal const bool CallReverseTrue = true;
        internal const bool CallReverseFalse = false;

        /// <summary>
        /// Global setting for AutoInitialize
        /// </summary>
        public static bool AutoInitialize { get; set; }

        /// <summary>
        /// Global setting for Default Mapping Direction
        /// </summary>
        public static MappingDirection DefaultMappingDirection
        {
            get { return _defaultMappingDirection; }
            set
            {
                ValidateMappingDirection(value);
                _defaultMappingDirection = value;
            }
        }

        /// <summary>
        /// Enumeration of mapping direction
        /// </summary>
        public enum MappingDirection
        {
            Unknown = 0,
            Bidirectional,
            LeftToRight
        }

        /// <summary>
        /// Validate that mapping direction is a valid value
        /// </summary>
        /// <param name="direction"></param>
        private static void ValidateMappingDirection(MappingDirection direction)
        {
            switch (direction)
            {
                case MappingDirection.Bidirectional:
                case MappingDirection.LeftToRight:
                    break;
                case MappingDirection.Unknown:
                default:
                    throw new ArgumentException("MappingDirection may not be set to this value", "direction");
            }
        }

        /// <summary>
        /// Retrieve a fluent object to define and execute mappings
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <returns></returns>
        public static fFastMapperFluent<TLeft, TRight> MapperFor<TLeft, TRight>()
        {
            return fFastMapperInternal<TLeft, TRight>.fFastMapFluent;
        }


        /// <summary>
        /// Retrieve a fluent object to define and execute mappings.  Parameters are ignored and used only for typing
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static fFastMapperFluent<TLeft, TRight> MapperFor<TLeft, TRight>(TLeft left, TRight right)
        {
            return fFastMapperInternal<TLeft, TRight>.fFastMapFluent;
        }

        /// <summary>
        /// Perform a mapping between types based on the defined maps for that type
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static TTo Map<TFrom, TTo>(TFrom from, TTo to)
        {
            return fFastMapperInternal<TFrom, TTo>.fFastMapFluent.Map(from, to);
        }

        /// <summary>
        /// Return a global settings object for fluent configuration of global settings
        /// </summary>
        /// <returns></returns>
        public static fFastMapperGlobalSettingsFluent GlobalSettings()
        {
            return _globalSettings;
        }


        public class fFastMapException : Exception
        {
            public fFastMapException() : base() { }
            public fFastMapException(string message) : base(message) { }
            public fFastMapException(string message, Exception innerException) : base(message, innerException) { }

#if !PORTABLE
            public fFastMapException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif

        }
    }
}
