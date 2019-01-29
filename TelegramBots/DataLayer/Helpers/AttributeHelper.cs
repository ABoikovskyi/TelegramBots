using System.ComponentModel.DataAnnotations;

namespace DataLayer.Helpers
{
    public static class AttributeHelper
	{
		public static string GetDisplayName<T>(this T source)
		{
			return GetEnumAttribute<T, DisplayAttribute>(source)?.Name;
		}
		
        private static TAttribute GetEnumAttribute<T, TAttribute>(this T source)
            where TAttribute : class
        {
            var field = source.GetType().GetField(source.ToString());
            var attributes = (TAttribute[])field.GetCustomAttributes(typeof(TAttribute), false);

            return attributes.Length > 0 ? attributes[0] : null;
        }
    }
}