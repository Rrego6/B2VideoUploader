using B2VideoUploader.Model;

namespace B2VideoExplorer.Tests;

public class UnitTest1
{
    int testVal = 0;
    [Fact]
    public void Test1()
    {
        Action action = () => { Task.Delay(200).Wait();  testVal++;  };

        B2VideoUploader.Model.AsyncJobQueue j = new B2VideoUploader.Model.AsyncJobQueue();
        j.Enqueue(new Action[] { action, action, action, action });
        j.Enqueue(new Action[] { action, action});
        j.WaitForJobs();
        Assert.Equal(6, testVal);

    }
}