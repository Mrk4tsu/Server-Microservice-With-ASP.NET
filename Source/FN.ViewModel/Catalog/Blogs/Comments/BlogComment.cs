using Google.Cloud.Firestore;

namespace FN.ViewModel.Catalog.Blogs.Comments
{
    [FirestoreData]
    public class BlogComment
    {
        [FirestoreProperty]
        public string Id { get; set; }
        [FirestoreProperty]
        public int UserId { get; set; }
        [FirestoreProperty]
        public string UserName { get; set; }
        [FirestoreProperty]
        public string FullName { get; set; }
        [FirestoreProperty]
        public string Avatar { get; set; }
        [FirestoreProperty]
        public int BlogId { get; set; }
        [FirestoreProperty]
        public string Content { get; set; }
        [FirestoreProperty]
        public DateTime CreatedDate { get; set; }
        [FirestoreProperty]
        public DateTime UpdateDate { get; set; }
        [FirestoreProperty]
        public int LikeCount { get; set; }
        [FirestoreProperty]
        public int DislikeCount { get; set; }
        [FirestoreProperty]
        public bool Status { get; set; }
    }
    public class BlogCommentCreate
    {
        public int BlogId { get; set; }
        public string Content { get; set; }
    }
}
