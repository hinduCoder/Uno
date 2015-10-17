namespace System.Web.Mvc.Html
{
    public static class Help
    {
        public static MvcHtmlString If(bool condition, string data)
        {
            return condition ? MvcHtmlString.Create(data) : MvcHtmlString.Empty;
        }
    }
}