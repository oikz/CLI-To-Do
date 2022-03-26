using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CLI_To_Do.MicrosoftToDo;
using Microsoft.Graph;
using Moq;
using NUnit.Framework;

namespace CLI_To_Do.Tests;

public class TaskHelperTests {
    private readonly Mock<GraphServiceClient> _mockClient = new(new MockAuthProvider(), null);

    private class MockAuthProvider : IAuthenticationProvider {
        public Task AuthenticateRequestAsync(HttpRequestMessage request) {
            return Task.Run(() => { });
        }
    }

    [SetUp]
    public void Setup() {
        _mockClient.Setup(e =>
            e.Me.Request().GetAsync(new CancellationToken()).Result).Returns(
            new User {
                DisplayName = "Test User"
            });

        _mockClient.Setup(e =>
            e.Me.Todo.Lists.Request().GetAsync(new CancellationToken()).Result).Returns(new TodoListsCollectionPage {
            new() {
                Id = "TestListId"
            }
        });

        ToDoTaskHelper.GraphClient = _mockClient.Object;
    }

    [Test]
    public async Task GetMeAsyncTest_Valid() {
        var result = await ToDoTaskHelper.GetMeAsync();
        Assert.AreEqual("Test User", result.DisplayName);
    }

    [Test]
    public async Task GetMeAsyncTest_Invalid() {
        _mockClient.Setup(e =>
            e.Me.Request().GetAsync(new CancellationToken()).Result).Throws(new ServiceException(new Error()));

        Assert.Null(await ToDoTaskHelper.GetMeAsync());
    }

    [Test]
    public async Task GetListsTest() {
        var result = await ToDoTaskHelper.GetLists();
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("TestListId", result.CurrentPage.First().Id);
    }
}