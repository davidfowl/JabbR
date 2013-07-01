using System.Collections.Generic;
using System.Dynamic;

namespace JabbR.Services
{
    public class EmailTemplateModelWrapper : DynamicObject
    {
        private readonly IDictionary<string, object> _propertyMap;

        public EmailTemplateModelWrapper(IDictionary<string, object> propertyMap)
        {
            if (propertyMap == null)
            {
                throw new System.ArgumentNullException("propertyMap");
            }

            this._propertyMap = propertyMap;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _propertyMap.Keys;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            return binder != null && _propertyMap.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (binder != null)
            {
                _propertyMap[binder.Name] = value;

                return true;
            }

            return false;
        }
    }
}