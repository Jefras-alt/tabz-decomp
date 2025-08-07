using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : Singleton<ServiceLocator>
{
	private static Dictionary<object, IService> m_services;

	private bool m_isInitialized;

	private void Awake()
	{
		if (!m_isInitialized)
		{
			m_services = new Dictionary<object, IService>();
			m_isInitialized = true;
			RegisterService<BTService>();
			RegisterService<InventoryService>();
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		foreach (KeyValuePair<object, IService> service in m_services)
		{
			service.Value.Update();
		}
	}

	private void FixedUpdate()
	{
		foreach (KeyValuePair<object, IService> service in m_services)
		{
			service.Value.FixedUpdate();
		}
	}

	private void LateUpdate()
	{
		foreach (KeyValuePair<object, IService> service in m_services)
		{
			service.Value.LateUpdate();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		UnregisterService<InventoryService>();
	}

	public static T GetService<T>() where T : IService
	{
		try
		{
			return (T)m_services[typeof(T)];
		}
		catch (KeyNotFoundException)
		{
			Debug.LogError("The requested service is not registered!");
			return default(T);
		}
	}

	public static void RegisterService<T>() where T : IService
	{
		if (m_services.ContainsKey(typeof(T)))
		{
			Debug.LogWarning(string.Concat("Service ", typeof(T), " allready exists!"));
			return;
		}
		T val = Activator.CreateInstance<T>();
		m_services.Add(typeof(T), val);
		val.Initialize();
	}

	public static void UnregisterService<T>() where T : IService
	{
		if (m_services.ContainsKey(typeof(T)))
		{
			m_services[typeof(T)].Destroy();
			m_services.Remove(typeof(T));
		}
		else
		{
			Debug.LogWarning(string.Concat("Could not unregister service ", typeof(T), ". Could not be found!"));
		}
	}
}
