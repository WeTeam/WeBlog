using System.Security.Authentication;
using NUnit.Framework;
using Sitecore.Data;

namespace Sitecore.Modules.WeBlog.UnitTest.sitecore_modules.web.WeBlog
{
    [TestFixture]
    public class MetaBlogApiFixture
    {
        [Test]
        public void getUsersBlogs_Unauthenticated()
        {
            var api = CreateNonAuthenticatingApi();
            Assert.That(() => {
                api.getUsersBlogs("app", "user", "password");
            }, Throws.InstanceOf<InvalidCredentialException>());
        }

        [Test]
        public void getCategories_Unauthenticated()
        {
            var api = CreateNonAuthenticatingApi();
            Assert.That(() => {
                api.getCategories(ID.NewID.ToString(), "user", "password");
            }, Throws.InstanceOf<InvalidCredentialException>());
        }

        [Test]
        public void getRecentPosts_Unauthenticated()
        {
            var api = CreateNonAuthenticatingApi();
            Assert.That(() => {
                api.getRecentPosts(ID.NewID.ToString(), "user", "password", 5);
            }, Throws.InstanceOf<InvalidCredentialException>());
        }

        [Test]
        public void getTemplate_Unauthenticated()
        {
            var api = CreateNonAuthenticatingApi();
            Assert.That(() => {
                api.getTemplate("app", ID.NewID.ToString(), "user", "password", "none");
            }, Throws.InstanceOf<InvalidCredentialException>());
        }

        [Test]
        public void newPost_Unauthenticated()
        {
            var api = CreateNonAuthenticatingApi();
            Assert.That(() => {
                api.newPost(ID.NewID.ToString(), "user", "password", null, false);
            }, Throws.InstanceOf<InvalidCredentialException>());
        }

        [Test]
        public void editPost_Unauthenticated()
        {
            var api = CreateNonAuthenticatingApi();
            Assert.That(() => {
                api.editPost(ID.NewID.ToString(), "user", "password", null, false);
            }, Throws.InstanceOf<InvalidCredentialException>());
        }

        [Test]
        public void getPost_Unauthenticated()
        {

            var api = CreateNonAuthenticatingApi();
            Assert.That(() => {
                api.getPost(ID.NewID.ToString(), "user", "password");
            }, Throws.InstanceOf<InvalidCredentialException>());
        }

        [Test]
        public void deletePost_Unauthenticated()
        {
            var api = CreateNonAuthenticatingApi();
            Assert.That(() => {
                api.deletePost("app", ID.NewID.ToString(), "user", "password", false);
            }, Throws.InstanceOf<InvalidCredentialException>());
        }

        [Test]
        public void newMediaObject_Unauthenticated()
        {
            var api = CreateNonAuthenticatingApi();
            Assert.That(() => {
                api.newMediaObject(ID.NewID.ToString(), "user", "password", null);
            }, Throws.InstanceOf<InvalidCredentialException>());
        }

        private MetaBlogApi CreateNonAuthenticatingApi()
        {
            return new TestableMetaBlogApi((username, password) =>
            {
                throw new InvalidCredentialException("Invalid credentials. Access denied");
            });
        }
    }
}
