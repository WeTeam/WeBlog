using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Diagnostics;
using Sitecore.Resources.Media;

namespace Sitecore.Modules.WeBlog.Import.Providers
{
    public class MediaItemBasedProvider : IWpPostProvider
    {
        private readonly ID _mediaItemId;
        private readonly Database _db;

        /// <summary>
        /// Imports the media item.
        /// </summary>
        public MediaItemBasedProvider(ID mediaItemIdId, Database db)
        {
            _db = db;
            _mediaItemId = mediaItemIdId;
        }

        public List<WpPost> GetPosts(WpImportOptions options)
        {
            Item mediaInnerItem = _db.GetItem(_mediaItemId);
            if (mediaInnerItem == null)
            {
                Logger.Error(String.Format("Media item for import could not be found (id: {0}, db: {1})", _mediaItemId, _db.Name));
                return new List<WpPost>(0);
            }
            MediaItem mediaItem = new MediaItem(mediaInnerItem);

            XmlDocument xmdDoc = new XmlDocument();
            var mediaStream = MediaManager.GetMedia(mediaItem).GetStream();
            if (mediaStream == null || mediaStream.MimeType != "text/xml")
            {
                Logger.Error(String.Format("MediaStream for imported item is null or uploaded file has is incorrect format (id: {0}, db: {1})", _mediaItemId, _db.Name));
                return new List<WpPost>(0);
            }

            xmdDoc.Load(mediaStream.Stream);
            using (var nodeReader = new XmlNodeReader(xmdDoc))
            {
                nodeReader.MoveToContent();
                var xDocument = XDocument.Load(nodeReader);

                var posts = (from item in xDocument.Descendants("item")
                             select new WpPost(item, options)).ToList();
                return posts;
            }
        }
    }
}