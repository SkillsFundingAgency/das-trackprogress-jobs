using SFA.DAS.TrackProgress.Jobs.MockServer;

    OuterApiBuilder.Create(5189)
        .WithKsbs()
        .WithNewProgress()
        .Build();

    Console.WriteLine("Press any key to stop the servers");
    Console.ReadKey();