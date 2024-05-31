using System.ComponentModel.DataAnnotations;

namespace QuanLyKho.Helper
{
    public class EnumHelper
    {

        public static string GetDisplayName(Enum enumValue)
        {
            var memberInfo = enumValue.GetType().GetMember(enumValue.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);

            if (attributes.Length > 0)
            {
                return ((DisplayAttribute)attributes[0]).Name;
            }

            return enumValue.ToString();
        }
    }
}
