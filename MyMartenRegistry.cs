using Marten;

namespace MartenDemo;

public class MyMartenRegistry : MartenRegistry
{
    public MyMartenRegistry()
    {
        base.For<UserTest>().DocumentAlias("user");
    }
}