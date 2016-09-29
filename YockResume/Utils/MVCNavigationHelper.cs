using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace YockResume.Utils
{
    public static class MVCNavigationHelper
    {
        public static string CurrentController(this HtmlHelper helper)
        {
            return helper.ViewContext.Controller.ControllerContext.RouteData.GetRequiredString("controller");
        }

        public static string CurrentAction(this HtmlHelper helper)
        {
            return helper.ViewContext.Controller.ControllerContext.RouteData.GetRequiredString("action");
        }

        public static bool IsCurrentController(this HtmlHelper helper, string testController)
        {
            return helper.CurrentController().Equals(testController, StringComparison.OrdinalIgnoreCase);
        }
        public static bool IsCurrentController(this HtmlHelper helper, string[] testController)
        {
            foreach (string controller in testController)
            {
                if (helper.CurrentController().Equals(controller, StringComparison.OrdinalIgnoreCase)) { return true; }
            }
            return false;
        }

        public static bool IsCurrentControllerAction(this HtmlHelper helper, string testController, string testAction)
        {
            return helper.CurrentController().Equals(testController, StringComparison.OrdinalIgnoreCase);
        }


        public static string NavItem(this HtmlHelper helper, string controllerIn)
        {
            return helper.NavItem(controllerIn, controllerIn, "Index", () => helper.IsCurrentController(controllerIn));
        }

        public static string NavItem(this HtmlHelper helper, string text, string controllerIn, string actionIn)
        {
            return helper.NavItem(controllerIn, controllerIn, actionIn, () => helper.IsCurrentController(controllerIn));
        }

        public static string NavItem(this HtmlHelper helper, string text, string controllerIn, string actionIn, Func<bool> isActive)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            var url = urlHelper.RouteUrl(new { controller = controllerIn, action = actionIn });

            var active = isActive();

            return NavItem(text, url, active, controllerIn.ToLower());
        }

        public static string SubNavItem(this HtmlHelper helper, string text, string controllerIn, string actionIn, Func<bool> isActive)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            var url = urlHelper.RouteUrl(new { controller = controllerIn, action = actionIn });

            var active = isActive();
            var extraCss = string.Format("{0}_{1}", controllerIn, actionIn).ToLower();

            return NavItem(text, url, active, extraCss);
        }

        private static string NavItem(string text, string url, bool active, string extraCssClass)
        {
            return string.Format("<li class='{0}'><a title='{1}' href='{2}'><span>{3}</span></a></li>",
                                 (active ? "active " : string.Empty) + extraCssClass,
                                 text,
                                 url,
                                 text);
        }

        public static string LoginStatusNavItem(this HtmlHelper helper)
        {
            if (HttpContext.Current.Request.IsAuthenticated)
                return helper.NavItem("Log Out", "Account", "Logout");

            return helper.NavItem("Login", "Account", "Login");
        }

        public static string SubNavListItems(this HtmlHelper helper)
        {
            var sb = new StringBuilder();
            var worker = ControllerNavigationItemCollection.Current;
            var name = helper.ViewContext.Controller.GetType().Name;

            if (worker.Controllers.ContainsKey(name))
            {
                var items = worker.Controllers[name].OrderBy(i => i.SortOrder);

                foreach (var navItem in items)
                {
                    var item = navItem;

                    if (navItem.IsSecure)
                    {
                        if (HttpContext.Current.Request.IsAuthenticated)
                        {
                            sb.Append(helper.SubNavItem(navItem.Text, navItem.Controller, navItem.Action, () => helper.IsCurrentControllerAction(item.Controller, item.Action)));
                        }
                    }
                    else
                    {
                        sb.Append(helper.SubNavItem(navItem.Text, navItem.Controller, navItem.Action, () => helper.IsCurrentControllerAction(item.Controller, item.Action)));
                    }
                }
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// This Attribute should go on Controller Actions
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    internal sealed class NavigationItemAttribute : Attribute
    {
        public NavigationItemAttribute(string text)
            : this(text, 0) { }

        public NavigationItemAttribute(string text, int order)
        {
            Text = text;
            SortOrder = order;
        }

        public string Text { get; private set; }
        public int SortOrder { get; set; }
    }

    /// <summary>
    /// POCO class for storage of reflection findings.
    /// </summary>
    public class ControllerNavigationItem
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public int SortOrder { get; set; }
        public string Text { get; set; }
        public bool IsSecure { get; set; }
    }

    /// <summary>
    /// Singleton object to load controllers and actions into a hashtable for lookup later.
    /// </summary>
    public sealed class ControllerNavigationItemCollection
    {
        static ControllerNavigationItemCollection _instance;
        static readonly object padlock = new object();

        public IDictionary<string, IEnumerable<ControllerNavigationItem>> Controllers { get; private set; }

        ControllerNavigationItemCollection()
        {
            Controllers = new Dictionary<string, IEnumerable<ControllerNavigationItem>>();
            PopulateCollection();
        }

        private void PopulateCollection()
        {
            var asm = Assembly.GetExecutingAssembly();
            var controllers = (from t in asm.GetTypes()
                               where
                                   typeof(Controller).IsAssignableFrom(t) &&
                                   typeof(Controller) != t
                               select t).ToList();

            controllers.ForEach(t => Controllers.Add(t.Name, GetControllerNavItems(t)));
        }

        private static IEnumerable<ControllerNavigationItem> GetControllerNavItems(Type controllerType)
        {
            var controllerDescriptor = new ReflectedControllerDescriptor(controllerType);

            var actions = (from a in controllerDescriptor.GetCanonicalActions()
                           let subNavAttr = (NavigationItemAttribute)a.GetCustomAttributes(typeof(NavigationItemAttribute), false).SingleOrDefault()
                           let authorize = a.GetCustomAttributes(typeof(AuthorizeAttribute), false).SingleOrDefault()
                           where a.IsDefined(typeof(NavigationItemAttribute), false)
                           select new ControllerNavigationItem
                           {
                               Action = a.ActionName,
                               Controller = a.ControllerDescriptor.ControllerName,
                               IsSecure = authorize != null,
                               SortOrder = subNavAttr.SortOrder,
                               Text = subNavAttr.Text
                           }).AsEnumerable();

            return actions;
        }

        public static ControllerNavigationItemCollection Current
        {
            get
            {
                lock (padlock)
                {
                    return _instance ?? (_instance = new ControllerNavigationItemCollection());
                }
            }
        }
    }
}