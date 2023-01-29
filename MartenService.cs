using System.Text.Json;
using Marten;

namespace MartenDemo;

public class MartenService : IMartenService
{
    private readonly IDocumentStore _store;

    public MartenService(IDocumentStore store)
    {
        _store = store;
    }

    public async Task DoWorkAsync()
    {
        await using (IDocumentSession session = _store.LightweightSession())
        {
            for (int i = 0; i < 50000; i++)
            {
                UserTest userTest = new UserTest { FirstName = $"Han {i}", LastName = $"Solo {1}" };
                session.Store(userTest);
            }

            await session.SaveChangesAsync();
        }

        await using (IQuerySession session = _store.QuerySession())
        {
            JsonSerializerOptions optins = new()
            {
                WriteIndented = true
            };

            IReadOnlyList<UserTest> existing = await session.Query<UserTest>().ToListAsync();

            foreach (UserTest user in existing)
            {
                Console.WriteLine(JsonSerializer.Serialize(user, optins));
            }
        }

        await using (IQuerySession session = _store.QuerySession())
        {
            Guid userId = Guid.Parse("0185fde1-42a9-45f0-b4e1-48a029eed992");

            IReadOnlyList<UserTest> existing = await session.Query<UserTest>().ToListAsync();

            UserTest? user = await session.LoadAsync<UserTest>(userId);
        }

        Guid questId = Guid.NewGuid();
        await using (IDocumentSession session = _store.OpenSession())
        {
            QuestStarted started = new QuestStarted { Name = "Destroy the One Ring" };
            MembersJoined joined1 = new MembersJoined(1, "Hobbiton", "Frodo", "Sam");

            // Start a brand new stream and commit the new events as
            // part of a transaction
            session.Events.StartStream<Quest>(questId, started, joined1);

            // Append more events to the same stream
            MembersJoined joined2 = new MembersJoined(3, "Buckland", "Merry", "Pippen");
            MembersJoined joined3 = new MembersJoined(10, "Bree", "Aragorn");
            ArrivedAtLocation arrived = new ArrivedAtLocation { Day = 15, Location = "Rivendell" };
            session.Events.Append(questId, joined2, joined3, arrived);

            // Save the pending changes to db
            await session.SaveChangesAsync();
        }

    }
}