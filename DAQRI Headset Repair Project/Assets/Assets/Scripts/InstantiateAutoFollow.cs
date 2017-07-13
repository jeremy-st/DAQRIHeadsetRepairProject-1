using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Threading;
using System.ComponentModel;
using UnityEngine.UI;

public class InstantiateAutoFollow : MonoBehaviour, IPointerClickHandler {

    public GameObject Content;
    Job myJob;


    //DAQRI.BodySpace bodySpace = new DAQRI.BodySpace();
    public void OnPointerClick(PointerEventData eventData)
    {
        myJob = new Job();
        myJob.GO = Content;
        myJob.Start();

    }


    public class ThreadedJob
    {
        private bool m_IsDone = false;
        private object m_Handle = new object();
        private Thread m_Thread = null;
        public bool IsDone
        {
            get
            {
                bool tmp;
                lock (m_Handle)
                {
                    tmp = m_IsDone;
                }
                return tmp;
            }
            set
            {
                lock (m_Handle)
                {
                    m_IsDone = value;
                }
            }
        }

        public virtual void Start()
        {
            m_Thread = new System.Threading.Thread(Run);
            m_Thread.Start();
        }
        public virtual void Abort()
        {
            m_Thread.Abort();
        }

        protected virtual void ThreadFunction() { }

        protected virtual void OnFinished() { }

        public virtual bool Update()
        {
            if (IsDone)
            {
                OnFinished();
                return true;
            }
            return false;
        }
        public IEnumerator WaitFor()
        {
            while (!Update())
            {
                yield return null;
            }
        }
        private void Run()
        {
            ThreadFunction();
            IsDone = true;
        }
    }
 public class Job : ThreadedJob
    {
        public GameObject GO;
        protected override void ThreadFunction()
        {
                MyDelay(3);
                
            
        }
        protected override void OnFinished()
        {
            addComponent(GO);
            
        }
    }

    void Update()
    {
        if (myJob != null)
        {
            if (myJob.Update())
            {
                // Alternative to the OnFinished callback
                myJob = null;
            }
        }
    }

    public static void addComponent(GameObject x)
    {  
        //x.AddComponent<DAQRI.BodySpace>();
    }

    public static void MyDelay(int seconds)
    {
        DateTime dt = DateTime.Now + TimeSpan.FromSeconds(seconds);
        
        do { } while (DateTime.Now < dt);
    }
}
