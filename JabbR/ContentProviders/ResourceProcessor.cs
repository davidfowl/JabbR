﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace JabbR.ContentProviders
{
    public class ResourceProcessor : IResourceProcessor
    {
        private readonly Lazy<IList<IContentProvider>> _contentProviders = new Lazy<IList<IContentProvider>>(GetContentProviders);

        public Task<string> ExtractResource(string url)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            var requestTask = Task.Factory.FromAsync((cb, state) => request.BeginGetResponse(cb, state), ar => request.EndGetResponse(ar), null);
            return requestTask.ContinueWith(task => ExtractContent((HttpWebResponse)task.Result));
        }

        private readonly static string contentWrapperFormat = "<div class='provided-content {0}'>{1}</div>";

        private string ExtractContent(HttpWebResponse response)
        {
            var item = (from i in
                            from p in _contentProviders.Value
                            select new
                            {
                                Name = p.Name,
                                Content = p.GetContent(response)
                            }
                        where i.Content != null
                        select i).FirstOrDefault();
            return item == null ? string.Empty : string.Format(contentWrapperFormat, item.Name, item.Content);
        }


        private static IList<IContentProvider> GetContentProviders()
        {
            // Use MEF to locate the content providers in this assembly
            var compositionContainer = new CompositionContainer(new AssemblyCatalog(typeof(ResourceProcessor).Assembly));
            return compositionContainer.GetExportedValues<IContentProvider>().ToList();
        }
    }
}