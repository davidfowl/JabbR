
namespace JabbR.ContentProviders
{
    public struct ContentProviderResultModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public static ContentProviderResultModel Empty = new ContentProviderResultModel();

        public override bool Equals(object obj)
        {
            if (!(obj is ContentProviderResultModel))
                return false;
            var toCompare = (ContentProviderResultModel)obj;
            return toCompare.Content == this.Content &&
                toCompare.Title == this.Title;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(ContentProviderResultModel left, ContentProviderResultModel right)
        {
            return left.Content == right.Content && left.Title == right.Title;
        }

        public static bool operator !=(ContentProviderResultModel left, ContentProviderResultModel right)
        {
            return left.Content != right.Content || left.Title != right.Title;
        }
    }
}