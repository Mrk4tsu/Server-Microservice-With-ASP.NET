namespace FN.Utilities
{
    public struct SystemConstant
    {
        public const string DB_CONNECTION_STRING = "MySQL";
        public const string REDIS_CONNECTION_STRING = "Redis";

        public const string MONGODB_SETTING = "MongoDBSettings";
        public const string SMTP_SETTINGS = "MailJet";
        public const string CLOUDINARY_SETTINGS = "Cloudinary";
        public const string FIREBASE_SETTINGS = "Firebase";

        public const string MESSAGE_PATTERN_EVENT = "myChannel:*";
        public const string MESSAGE_REGISTER_EVENT = "myChannel:RegisterEvent";
        public const string MESSAGE_LOGIN_EVENT = "myChannel:LoginEvent";
        public const string MESSAGE_UPDATE_EMAIL_EVENT = "myChannel:EmailEvent";
        public const string MESSAGE_FORGOT_PASSWORD_EVENT = "myChannel:ForgotPasswordEvent";
        public const string MESSAGE_CONFIRM_ACCOUNT_EVENT = "myChannel:ConfirmAccountEvent";

        public const string TEMPLATE_ORDER_ID = "6705985";
        public const string TEMPLATE_WELCOME_ID = "6710966";
        public const string TEMPLATE_WARNING_ID = "6710973";
        public const string TEMPLATE_UPDATE_MAIL_ID = "6716002";
        public const string TEMPLATE_RESET_PASSWORD_ID = "6718673";
        public const string TEMPLATE_CONFIRM_ACCOUNT = "6890902";

        public const string PRODUCT_KEY = "product";
        public const string CATEGORY_KEY = "category";
        public const string BLOG_KEY = "blog";
        public const string BLOG_DETAIL_KEY = "blog";
        public const string USER_KEY = "user";
        public static readonly string CACHE_USER_BY_USERNAME = "user-cache:";

        public const string EVENT_PAYMENT_GROUP_KAFKA = "event-payment-group";
        public const string EVENT_PAYMENT_TOPIC_KAFKA = "event-payment";

        public const string AVATAR_DEFAULT = "https://res.cloudinary.com/dje3seaqj/image/upload/v1736989161/gatapchoi_biglrl.jpg";

    }
}
