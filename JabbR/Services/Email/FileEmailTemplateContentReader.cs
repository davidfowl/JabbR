using System;
using System.Globalization;
using System.IO;

namespace JabbR.Services
{
    public class FileEmailTemplateContentReader : IEmailTemplateContentReader
    {
        public FileEmailTemplateContentReader()
            : this("views/emailtemplates", ".cshtml")
        {
        }

        public FileEmailTemplateContentReader(string templateDirectory, string fileExtension)
        {
            if (string.IsNullOrEmpty(templateDirectory))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, "\"{0}\" cannot be blank.", "templateDirectory"));
            }

            if (!Path.IsPathRooted(templateDirectory))
            {
                templateDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, templateDirectory);
            }

            if (!Directory.Exists(templateDirectory))
            {
                throw new DirectoryNotFoundException(string.Format(CultureInfo.CurrentCulture, "\"{0}\" does not exist.", templateDirectory));
            }

            TemplateDirectory = templateDirectory;
            FileExtension = fileExtension;
        }

        protected string TemplateDirectory { get; private set; }

        protected string FileExtension { get; private set; }

        public string Read(string templateName, string suffix)
        {
            if (string.IsNullOrEmpty(templateName))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, "\"{0}\" cannot be blank.", "templateName"));
            }

            var content = string.Empty;
            var path = BuildPath(templateName, suffix);

            if (File.Exists(path))
            {
                content = File.ReadAllText(path);
            }

            return content;
        }

        protected virtual string BuildPath(string templateName, string suffix)
        {
            var fileName = templateName;

            if (!string.IsNullOrWhiteSpace(suffix))
            {
                fileName += "." + suffix;
            }

            if (!string.IsNullOrWhiteSpace(FileExtension))
            {
                fileName += FileExtension;
            }

            var path = Path.Combine(TemplateDirectory, fileName);

            return path;
        }
    }
}