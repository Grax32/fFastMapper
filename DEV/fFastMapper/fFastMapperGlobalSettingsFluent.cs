using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grax.fFastMapper
{
    public class fFastMapperGlobalSettingsFluent
    {
        /// <summary>
        /// Internal constructor
        /// </summary>
        internal fFastMapperGlobalSettingsFluent() { }

        public fFastMapperGlobalSettingsFluent SetDefaultMappingDirection(fFastMap.MappingDirection mappingDirection)
        {
            fFastMap.DefaultMappingDirection = mappingDirection;
            return this;
        }

        public fFastMapperGlobalSettingsFluent SetAutoInitialize(bool doAutoInitialize)
        {
            fFastMap.AutoInitialize = doAutoInitialize;
            return this;
        }
    }
}
