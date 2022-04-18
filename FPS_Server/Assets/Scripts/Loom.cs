using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

public class Loom : MonoBehaviour
{
	public static int maxThreads = 8;//����߳�����
	static int numThreads;

	private static Loom _current;
	private int _count;
	//��ȡLoom
	public static Loom Current
	{
		get
		{
			//��ʼ�� ����_current��loom��
			Initialize();
			return _current;
		}
	}

	void Awake()
	{
		//��ʼ��������ק�ű���
		_current = this;
		initialized = true;
	}

	static bool initialized;//true �ѳ�ʼ��

	//�ڳ�������ʱ������һ��������loom�������ֹ������ק�ű���
	static void Initialize()
	{
		//����
		if (!initialized)
		{
			//��������ʱִ�У�����һ��Loom���
			if (Application.isPlaying)
			{
				initialized = true;
				var g = new GameObject("Loom");
				_current = g.AddComponent<Loom>();
			}
		}
	}

	private List<Action> _actions = new List<Action>();//��Ϊ�б�
													   //��ʱ�Ŷ�
	public struct DelayedQueueItem
	{
		public float time;//��ʱ
		public Action action;//��Ϊ
	}
	private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();//��ʱ��Ϊ�б�

	List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();


	#region �����̵߳��÷���
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
		//��ǰʱ�䲻Ϊ0 д����ʱ�б���
		if (time != 0)
		{
			//��ֹ��������ֹ_delayed������д���ʱ���������̽��������������´���
			lock (Current._delayed)
			{
				Current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
			}
		}
		else
		{
			//��Ϊ0 д����Ϊ�б���
			lock (Current._actions)
			{
				Current._actions.Add(action);
			}
		}
	}

	#endregion

	#region ���̵߳��õķ���
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
		//��ǰloom���
		if (_current == this)
		{
			_current = null;
		}
	}



	// Use this for initialization
	void Start()
	{

	}

	List<Action> _currentActions = new List<Action>();//��ǰ��Ϊ�б�

	// Update is called once per frame
	void Update()
	{
		//��Ϊ�б� ������
		lock (_actions)
		{
			_currentActions.Clear();//��ǰ��Ϊ�б� ���
			_currentActions.AddRange(_actions);//��_actions�б�ȫ����ӵ�_currentActions�б���
			_actions.Clear();//_actions�б����
		}
		foreach (var a in _currentActions)
		{
			a();//ѭ��ִ�� a ��Ϊ
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