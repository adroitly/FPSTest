using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

public class Loom : MonoBehaviour
{
	public static int maxThreads = 8;//最大线程数量
	static int numThreads;

	private static Loom _current;
	private int _count;
	//获取Loom
	public static Loom Current
	{
		get
		{
			//初始化 返回_current（loom）
			Initialize();
			return _current;
		}
	}

	void Awake()
	{
		//初始化（已拖拽脚本）
		_current = this;
		initialized = true;
	}

	static bool initialized;//true 已初始化

	//在程序运行时，创建一个单例的loom组件（防止忘记拖拽脚本）
	static void Initialize()
	{
		//单例
		if (!initialized)
		{
			//程序运行时执行，创建一个Loom组件
			if (Application.isPlaying)
			{
				initialized = true;
				var g = new GameObject("Loom");
				_current = g.AddComponent<Loom>();
			}
		}
	}

	private List<Action> _actions = new List<Action>();//行为列表
													   //延时排队
	public struct DelayedQueueItem
	{
		public float time;//延时
		public Action action;//行为
	}
	private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();//延时行为列表

	List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();


	#region 在主线程调用方法
	public static void QueueOnMainThread(Action action)
	{
		QueueOnMainThread(action, 0f);
	}
	public static void LogOnMainThread(string text)
    {
		QueueOnMainThread(() => { Debug.Log(text); });

	}
	public static void QueueOnMainThread(Action action, float time)
	{
		//当前时间不为0 写入延时列表中
		if (time != 0)
		{
			//防止锁死（防止_delayed集合在写入的时候，其他进程进行其他操作导致错误）
			lock (Current._delayed)
			{
				Current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
			}
		}
		else
		{
			//不为0 写入行为列表中
			lock (Current._actions)
			{
				Current._actions.Add(action);
			}
		}
	}

	#endregion

	#region 在线程调用的方法
	public static Thread RunAsync(Action a)
	{
		Initialize();
		while (numThreads >= maxThreads)
		{
			Thread.Sleep(1);
		}
		Interlocked.Increment(ref numThreads);
		ThreadPool.QueueUserWorkItem(RunAction, a);
		return null;
	}

	private static void RunAction(object action)
	{
		try
		{
			((Action)action)();
		}
		catch
		{
		}
		finally
		{
			Interlocked.Decrement(ref numThreads);
		}

	}
	#endregion


	void OnDisable()
	{
		//当前loom清空
		if (_current == this)
		{
			_current = null;
		}
	}



	// Use this for initialization
	void Start()
	{

	}

	List<Action> _currentActions = new List<Action>();//当前行为列表

	// Update is called once per frame
	void Update()
	{
		//行为列表 防锁死
		lock (_actions)
		{
			_currentActions.Clear();//当前行为列表 清空
			_currentActions.AddRange(_actions);//将_actions列表全部添加到_currentActions列表中
			_actions.Clear();//_actions列表清空
		}
		foreach (var a in _currentActions)
		{
			a();//循环执行 a 行为
		}
		lock (_delayed)
		{
			_currentDelayed.Clear();
			_currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
			foreach (var item in _currentDelayed)
				_delayed.Remove(item);
		}
		foreach (var delayed in _currentDelayed)
		{
			delayed.action();
		}



	}
}