using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������е����ݶ���
/// </summary>
public class PoolData
{
    //�����洢���ݶ���
    private Stack<GameObject> dataStack = new Stack<GameObject>();
    //�����洢����ʹ�õ����ݶ���
    private Queue<GameObject> usedDataQueue = new Queue<GameObject>();

    //���������ʹ�õ����ݶ������������ֵ �ӹ��ص������������ò���ȡ
    public int maxNum;
    //��������� �������в��ֹ���
    private GameObject rootObj;
    //��ȡ�������Ƿ��ж���
    public int Count => dataStack.Count;
    //��ȡ����ʹ�õ����ݶ�������
    public int usedCount => usedDataQueue.Count;

    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="poolObj">������Ϊ���Ķ���</param>
    /// <param name="rootName">�����������</param>
    /// <param name="firstObj">��һ����ȡ�Ķ���</param>
    public PoolData(GameObject poolObj,string rootName,GameObject firstObj)
    {
        //�������ֹ���ʱ �Żᶯ̬�������ӹ�ϵ
        if(PoolMgr.isOpenLayout)
        {
            //�������븸����
            rootObj = new GameObject(rootName + "_Pool");
            //�͹��Ӷ��������ӹ�ϵ
            rootObj.transform.SetParent(poolObj.transform);
        }
        //��gameObject���Ϲ��صĽű��ϻ�ȡ���Ƶ����ֵ
        //PoolObjMaxNum poolObjMaxNum = firstObj.GetComponent<PoolObjMaxNum>();
        //if(poolObjMaxNum == null)
        //{
        //    Debug.LogError("��Ϊ�������PoolObjMaxNum�ű����������������ֵ��");
        //    return;
        //}
        //maxNum = poolObjMaxNum.maxNum;
        //if(maxNum == 0)
        //{
        //    Debug.LogError("��Ϊ������ص�PoolObjMaxNum�ű��ϵ�maxNum�����������ֵ��");
        //    return;
        //}
        if(maxNum == 1)
        {
            Debug.LogError("�벻Ҫ���������ֵ��Ϊ1��");
            return;
        }
    }

    /// <summary>
    /// �ӻ������ȡ������
    /// </summary>
    /// <returns>��Ҫ�Ķ�������</returns>
    public GameObject Pop()
    {
        //ȡ������
        GameObject obj = dataStack.Pop();
        //�������
        obj.SetActive(true);
        //�������������ʹ�õĶ���
        usedDataQueue.Enqueue(obj);

        if (PoolMgr.isOpenLayout)
        {
            //�Ͽ����ӹ�ϵ
            obj.transform.SetParent(null);
        }

        return obj;
    }

    /// <summary>
    /// �������ŵ��������
    /// </summary>
    /// <param name="obj">Ҫ��ŵ�����</param>
    public void Push(GameObject obj)
    {
        //�ö���ʧ��
        obj.SetActive(false);
        //�ö����뿪����ʹ�õĶ���
        usedDataQueue.Dequeue();

        if (PoolMgr.isOpenLayout)
        {
            //�����Ӧ����ĸ����� �������ӹ�ϵ
            obj.transform.SetParent(rootObj.transform);
        }
        //��ջ��¼��Ӧ�Ķ�������
        dataStack.Push(obj);
    }

    public void usedQueuePush(GameObject obj)
    {
        usedDataQueue.Enqueue(obj);
    }

    public GameObject usedQueuePop()
    {
        GameObject obj = usedDataQueue.Dequeue();
        return obj;
    }
}

/// <summary>
/// ����أ�����أ�ģ�� ������
/// </summary>
public class PoolMgr : BaseManager<PoolMgr>
{
    //��������
    private Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
    //��������� �������в��ֹ���
    private GameObject poolObj;
    //�Ƿ������ֹ���
    public static bool isOpenLayout = true;

    private PoolMgr() { }

    /// <summary>
    /// �ӻ������ȡ�����ķ���
    /// </summary>
    /// <param name="name">��������������</param>
    /// <param name="maxNum">������������ Ĭ��Ϊ10</param>
    /// <returns>ȡ���Ķ���</returns>
    public GameObject GetObj(string name)
    {
        GameObject obj;

        //������ǰ�ж��Ƿ��и�����
        //��Ϊ�� �򴴽�
        if (PoolMgr.isOpenLayout && poolObj == null )
        {
            poolObj = new GameObject("Pool");
        }
        //�����ڶ�Ӧ�ĳ������� ��������
        if (!poolDic.ContainsKey(name))
        {
            //ͨ����Դ����ȥʵ����һ��gameobject
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            obj.name = name;
            //����һ���µĳ���
            poolDic.Add(name, new PoolData(poolObj, name ,obj));
            //��obj��������ʹ���е����ݶ������
            poolDic[name].usedQueuePush(obj);
        }
        //���ڶ�Ӧ�ĳ�������
        else
        {
            //��ʹ���еĶ���������������
            if (poolDic[name].usedCount >= poolDic[name].maxNum)
            {
                //��usedDataQueue��ȡ�������ٴμ���usedDataQueue��
                obj = poolDic[name].usedQueuePop();
                poolDic[name].usedQueuePush(obj);
            }
            //��ʹ���еĶ�������û��������
            else
            {
                //�������ж���
                if (poolDic[name].Count > 0)
                {
                    obj = poolDic[name].Pop();
                }
                //���û�ж��� ȥ����
                else
                {
                    //ͨ����Դ����ȥʵ����һ��gameobject
                    obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
                    obj.name = name;
                    poolDic[name].usedQueuePush(obj);   
                }
            }
        }
        return obj;
    }

    /// <summary>
    /// ��������зŶ����ķ���
    /// </summary>
    /// <param name="name">������������</param>
    /// <param name="obj">����Ķ���</param>
    public void PushObj(GameObject obj)
    {
        //�������д������
        poolDic[obj.name].Push(obj);
    }
    /// <summary>
    /// ��������������ӵ�����
    /// ʹ�ó�����Ҫ���г���ʱ
    /// </summary>
    public void ClearPool()
    {
        poolDic.Clear();
        //�л�����ʱ ������ҲҪ���Ƴ�
        poolObj = null;
    }
}
