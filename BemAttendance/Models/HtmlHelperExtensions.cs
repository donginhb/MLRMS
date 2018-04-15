using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEMAttendance.Models
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString CreatePageLiTag(this UrlHelper urlHelper,PageModel pageModel,int index,bool isCurrentIndex=false,bool isDisable=true,string content="")
        {
            string url = urlHelper.Action(pageModel.ActionName, new { searchKey = pageModel.SearchKeyWord, dptName1List = pageModel.DptSelectCode, regStatus = pageModel.RegStatus,searchType =pageModel.SearchType,index = index,id=pageModel.id});
            string activeClass = !isCurrentIndex ? string.Empty : "class='active'";
            string disableClass = isDisable ? string.Empty : "class='disabled'";
            url = isDisable ? "href='" + url + "'" : string.Empty;
            string contentString = string.IsNullOrEmpty(content) ? index.ToString() : content;
            return new MvcHtmlString("<li " + activeClass + disableClass + "><a " + url + ">" + contentString + "</a></li>");
        }
        public static MvcHtmlString Disable(this MvcHtmlString helper, bool disabled)
        {
            if (helper == null)
                throw new ArgumentNullException();

            if (disabled)
            {
                string html = helper.ToString();
                int startIndex = html.IndexOf('>');

                html = html.Insert(startIndex, " disabled=\"disabled\"");
                return MvcHtmlString.Create(html);
            }

            return helper;
        }
        public static MvcHtmlString CreatePageLiTagV2(this UrlHelper urlHelper, PageModelV2 pageModel, int index, bool isCurrentIndex = false, bool isDisable = true, string content = "")
        {
            string url = string.Empty;
            url = urlHelper.Action(pageModel.ActionName, new { searchKey = pageModel.SearchKeyWord, beginTime1 = pageModel.BeginTime, endTime1 = pageModel.EndTime, dptName1List = pageModel.DptName,index = index });
       

            string activeClass = !isCurrentIndex ? string.Empty : "class='active'";
            string disableClass = isDisable ? string.Empty : "class='disabled'";
            url = isDisable ? "href='" + url + "'" : string.Empty;
            string contentString = string.IsNullOrEmpty(content) ? index.ToString() : content;
            return new MvcHtmlString("<li " + activeClass + disableClass + "><a " + url + ">" + contentString + "</a></li>");
        }
    }
}