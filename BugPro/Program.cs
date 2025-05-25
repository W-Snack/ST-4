using Stateless;
using System;

public class Bug
{
    public enum State { Open, Assigned, Defered, Closed, Testing, Resolved, Reopened }
    private enum Trigger { Assign, Defer, Close, Test, Resolve, Reopen }

    private StateMachine<State, Trigger> sm;

    public Bug(State state)
    {
        sm = new StateMachine<State, Trigger>(state);

        sm.Configure(State.Open)
            .Permit(Trigger.Assign, State.Assigned);

        sm.Configure(State.Assigned)
            .Permit(Trigger.Close, State.Closed)
            .Permit(Trigger.Defer, State.Defered)
            .Permit(Trigger.Test, State.Testing)
            .Ignore(Trigger.Assign);

        sm.Configure(State.Defered)
            .Permit(Trigger.Assign, State.Assigned);

        sm.Configure(State.Closed)
            .Permit(Trigger.Reopen, State.Reopened);

        sm.Configure(State.Testing)
            .Permit(Trigger.Resolve, State.Resolved)
            .Permit(Trigger.Assign, State.Assigned);

        sm.Configure(State.Resolved)
            .Permit(Trigger.Close, State.Closed)
            .Permit(Trigger.Reopen, State.Reopened);

        sm.Configure(State.Reopened)
            .Permit(Trigger.Assign, State.Assigned);
    }

    public void Close()
    {
        sm.Fire(Trigger.Close);
        Console.WriteLine("Close");
    }

    public void Assign()
    {
        sm.Fire(Trigger.Assign);
        Console.WriteLine("Assign");
    }

    public void Defer()
    {
        sm.Fire(Trigger.Defer);
        Console.WriteLine("Defer");
    }

    public void Test()
    {
        sm.Fire(Trigger.Test);
        Console.WriteLine("Test");
    }

    public void Resolve()
    {
        sm.Fire(Trigger.Resolve);
        Console.WriteLine("Resolve");
    }

    public void Reopen()
    {
        sm.Fire(Trigger.Reopen);
        Console.WriteLine("Reopen");
    }

    public State GetState()
    {
        return sm.State;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var bug = new Bug(Bug.State.Open);
        bug.Assign();
        bug.Test();
        bug.Resolve();
        bug.Close();
        bug.Reopen();
        bug.Assign();
        bug.Defer();
        bug.Assign();
        Console.WriteLine(bug.GetState());
    }
}