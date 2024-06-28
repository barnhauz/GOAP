﻿using System.Collections.Generic;
using CrashKonijn.Goap.Classes;
using CrashKonijn.Goap.Classes.Runners;
using CrashKonijn.Goap.Core.Interfaces;
using CrashKonijn.Goap.Resolver;
using CrashKonijn.Goap.Resolver.Interfaces;
using CrashKonijn.Goap.UnitTests.Support;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using UnityEngine;
using ICondition = CrashKonijn.Goap.Core.Interfaces.ICondition;

namespace CrashKonijn.Goap.UnitTests
{
    public class AgentTypeJobRunnerTests
    {
        private IMonoAgent agent;
        
        [SetUp]
        public void Setup()
        {
            this.agent = Substitute.For<IMonoAgent>();
            this.agent.Position.Returns(Vector3.zero);
        }
        
        [Test]
        public void Run_UpdatesSensorRunner()
        {
            // Arrange
            var sensorRunner = Substitute.For<ISensorRunner>();
            var agentType = Substitute.For<IAgentType>();
            agentType.SensorRunner.Returns(sensorRunner);
            agentType.GetAllNodes().Returns(new List<IConnectable>());
            agentType.GoapConfig.Returns(GoapConfig.Default);

            var resolver = Substitute.For<IGraphResolver>();
            
            var runner = new AgentTypeJobRunner(agentType, resolver);
            
            // Act
            runner.Run(new HashSet<IMonoGoapAgent>() { this.agent });
            runner.Dispose();
            
            // Assert
            sensorRunner.Received(1).Update();
        }
        
        [Test]
        public void Run_WithNoAgens_SensesGlobalSensors()
        {
            // Arrange
            var sensorRunner = Substitute.For<ISensorRunner>();
            var agentType = Substitute.For<IAgentType>();
            agentType.SensorRunner.Returns(sensorRunner);
            agentType.GetAllNodes().Returns(new List<IConnectable>());
            agentType.GoapConfig.Returns(GoapConfig.Default);
            
            var resolver = Substitute.For<IGraphResolver>();
            
            var runner = new AgentTypeJobRunner(agentType, resolver);
            
            // Act
            runner.Run(new HashSet<IMonoGoapAgent>());
            runner.Dispose();
            
            // Assert
            sensorRunner.Received(1).SenseGlobal();
        }
        
        [Test]
        public void Run_WithAtLeastOneAgent_SensesGlobalSensors()
        {
            // Arrange
            var sensorRunner = Substitute.For<ISensorRunner>();
            var agentType = Substitute.For<IAgentType>();
            agentType.SensorRunner.Returns(sensorRunner);
            agentType.GetAllNodes().Returns(new List<IConnectable>());
            agentType.GoapConfig.Returns(GoapConfig.Default);
            
            var resolver = Substitute.For<IGraphResolver>();
            
            var runner = new AgentTypeJobRunner(agentType, resolver);
            
            // Act
            runner.Run(new HashSet<IMonoGoapAgent>() { this.agent });
            runner.Dispose();
            
            // Assert
            sensorRunner.Received(1).SenseGlobal();
        }

        [Test]
        public void Run_AgentHasNoCurrentGoal_DoesNotRun()
        {
            // Arrange
            agent.CurrentGoal.ReturnsNull();
            agent.Agent.ActionState.Action.ReturnsNull();
            
            var sensorRunner = Substitute.For<ISensorRunner>();
            
            var agentType = Substitute.For<IAgentType>();
            agentType.SensorRunner.Returns(sensorRunner);
            agentType.GetAllNodes().Returns(new List<IConnectable> { });
            agentType.GoapConfig.Returns(GoapConfig.Default);
            
            var resolver = Substitute.For<IGraphResolver>();
            
            var runner = new AgentTypeJobRunner(agentType, resolver);
            
            // Act
            runner.Run(new HashSet<IMonoGoapAgent>());
            runner.Dispose();
            
            // Assert
            sensorRunner.Received(0).SenseLocal(agent);
            resolver.Received(0).StartResolve(Arg.Any<RunData>());
        }

        [Test]
        public void Run_AgentHasCurrentGoalAndNoAction_Runs()
        {
            // Arrange
            var goal = Substitute.For<IGoal>();
            goal.Conditions.Returns(new [] { Substitute.For<ICondition>() });
            
            agent.CurrentGoal.Returns(goal);
            agent.Agent.ActionState.Action.ReturnsNull();
            
            var sensorRunner = Substitute.For<ISensorRunner>();
            
            var agentType = Substitute.For<IAgentType>();
            agentType.SensorRunner.Returns(sensorRunner);
            agentType.GetAllNodes().Returns(new List<IConnectable> { goal });
            agentType.GetActions().Returns(new List<IGoapAction>());
            
            var resolver = Substitute.For<IGraphResolver>();
            
            var runner = new AgentTypeJobRunner(agentType, resolver);
            
            // Act
            runner.Run(new HashSet<IMonoGoapAgent>() { agent });
            runner.Dispose();
            
            // Assert
            sensorRunner.Received(1).SenseLocal(this.agent);
            resolver.Received(1).StartResolve(Arg.Any<RunData>());
        }
        
        [Test]
        public void Run_AgentHasCurrentGoalAndDoesHaveAction_Runs()
        {
            // Arrange
            var goal = Substitute.For<IGoal>();
            goal.Conditions.Returns(new []{ Substitute.For<ICondition>() });

            this.agent.CurrentGoal.Returns(goal);
            this.agent.Agent.ActionState.Action.Returns(Substitute.For<IAction>());
            
            var sensorRunner = Substitute.For<ISensorRunner>();
            
            var agentType = Substitute.For<IAgentType>();
            agentType.SensorRunner.Returns(sensorRunner);
            agentType.GetAllNodes().Returns(new List<IConnectable> { goal });
            agentType.GetActions().Returns(new List<IGoapAction>());
            
            var resolver = Substitute.For<IGraphResolver>();
            
            var runner = new AgentTypeJobRunner(agentType, resolver);
            
            // Act
            runner.Run(new HashSet<IMonoGoapAgent>() { agent });
            runner.Dispose();
            
            // Assert
            sensorRunner.Received(1).SenseLocal(this.agent);
            resolver.Received(1).StartResolve(Arg.Any<RunData>());
        }

        [Test]
        public void Run_AgentHasCompletedGoal_CallsGoalCompleteEvent()
        {
            // Arrange
            var goal = Substitute.For<IGoal>();
            goal.Conditions.Returns(new [] { Substitute.For<ICondition>() });

            this.agent.CurrentGoal.Returns(goal);
            this.agent.Agent.ActionState.Action.ReturnsNull();
            
            var sensorRunner = Substitute.For<ISensorRunner>();
            
            var agentType = Substitute.For<IAgentType>();
            agentType.SensorRunner.Returns(sensorRunner);
            agentType.GetAllNodes().Returns(new List<IConnectable> { goal });
            agentType.GetActions().Returns(new List<IGoapAction>());

            agentType.GoapConfig.ConditionObserver.IsMet(Arg.Any<ICondition>()).Returns(true);
            
            var resolver = Substitute.For<IGraphResolver>();
            
            var runner = new AgentTypeJobRunner(agentType, resolver);
            
            // Act
            runner.Run(new HashSet<IMonoGoapAgent>() { agent });
            runner.Dispose();
            
            // Assert
            sensorRunner.Received(1).SenseLocal(this.agent);
            resolver.Received(0).StartResolve(Arg.Any<RunData>());
            this.agent.Events.Received(1).GoalCompleted(this.agent.CurrentGoal);
        }

        [Test]
        public void Run_AgentHasNotCompletedGoal_DoesntCallGoalCompleteEvent()
        {
            // Arrange
            var goal = Substitute.For<IGoal>();
            goal.Conditions.Returns(new [] { Substitute.For<ICondition>() });

            this.agent.CurrentGoal.Returns(goal);
            this.agent.Agent.ActionState.Action.ReturnsNull();
            
            var sensorRunner = Substitute.For<ISensorRunner>();
            
            var agentType = Substitute.For<IAgentType>();
            agentType.SensorRunner.Returns(sensorRunner);
            agentType.GetAllNodes().Returns(new List<IConnectable> { goal });
            agentType.GetActions().Returns(new List<IGoapAction>());

            agentType.GoapConfig.ConditionObserver.IsMet(Arg.Any<ICondition>()).Returns(false);
            
            var resolver = Substitute.For<IGraphResolver>();
            
            var runner = new AgentTypeJobRunner(agentType, resolver);
            
            // Act
            runner.Run(new HashSet<IMonoGoapAgent>() { agent });
            runner.Dispose();
            
            // Assert
            sensorRunner.Received(1).SenseLocal(this.agent);
            resolver.Received(1).StartResolve(Arg.Any<RunData>());
            this.agent.Events.Received(0).GoalCompleted(this.agent.CurrentGoal);
        }
        
        [Test]
        public void Run_AgentHasCurrentGoalAndNoAction_SetsTheActionOnAgent()
        {
            // Arrange
            var goal = Substitute.For<IGoal>();
            goal.Conditions.Returns(new [] { Substitute.For<ICondition>() });

            var action = Substitute.For<IGoapAction>();
            
            agent.CurrentGoal.Returns(goal);
            agent.Agent.ActionState.Action.ReturnsNull();
            
            var sensorRunner = Substitute.For<ISensorRunner>();
            
            var agentType = Substitute.For<IAgentType>();
            agentType.SensorRunner.Returns(sensorRunner);
            agentType.GetAllNodes().Returns(new List<IConnectable> { goal });
            agentType.GetActions().Returns(new List<IGoapAction>());

            var handle = Substitute.For<IResolveHandle>();
            handle.Complete().Returns(new IConnectable[] { action });
            
            var resolver = Substitute.For<IGraphResolver>();
            resolver.StartResolve(Arg.Any<RunData>()).Returns(handle);
            
            var runner = new AgentTypeJobRunner(agentType, resolver);
            
            // Act
            runner.Run(new HashSet<IMonoGoapAgent>() { agent });
            runner.Complete();
            
            // Assert
            this.agent.Received(1).SetAction(action, Arg.Any<IConnectable[]>(), Arg.Any<ITarget>());
        }
        
        [Test]
        public void Run_AgentHasCurrentGoalAndAction_ResolvingSameActionDoesntCallSet()
        {
            // Arrange
            var goal = Substitute.For<IGoal>();
            goal.Conditions.Returns(new [] { Substitute.For<ICondition>() });

            var action = Substitute.For<IGoapAction>();
            
            agent.CurrentGoal.Returns(goal);
            agent.Agent.ActionState.Action.Returns(action);
            
            var sensorRunner = Substitute.For<ISensorRunner>();
            
            var agentType = Substitute.For<IAgentType>();
            agentType.SensorRunner.Returns(sensorRunner);
            agentType.GetAllNodes().Returns(new List<IConnectable> { goal });
            agentType.GetActions().Returns(new List<IGoapAction>());

            var handle = Substitute.For<IResolveHandle>();
            handle.Complete().Returns(new IConnectable[] { action });
            
            var resolver = Substitute.For<IGraphResolver>();
            resolver.StartResolve(Arg.Any<RunData>()).Returns(handle);
            
            var runner = new AgentTypeJobRunner(agentType, resolver);
            
            // Act
            runner.Run(new HashSet<IMonoGoapAgent>() { agent });
            runner.Complete();
            
            // Assert
            agent.Received(0).SetAction(Arg.Any<IGoapAction>(), Arg.Any<IConnectable[]>(), Arg.Any<ITarget>());
        }
    }
}