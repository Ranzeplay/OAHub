using System;
using System.Collections.Generic;
using System.Text;

namespace OAHub.Base.Models.StatusModels
{
    public enum PostColor
    {
        Primary,
        Info,
        Success,
        Warning,
        Danger,
        Light,
        Dark
    }

    public class BulmaConfig
    {
        public static Dictionary<PostColor, string> BulmaColorTag = new Dictionary<PostColor, string>
        {
            {PostColor.Primary, "is-primary" },
            {PostColor.Info, "is-info" },
            {PostColor.Success, "is-success" },
            {PostColor.Warning, "is-warning" },
            {PostColor.Danger, "is-danger" },
            {PostColor.Light, "is-light" },
            {PostColor.Dark, "is-dark" },
        };
    }
}
