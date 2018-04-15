using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thrift.Protocol;
using Thrift.Transport;

namespace BEMAttendance.Models.Thrift
{
    public class ThriftClient
    {
        TTransport transport;
        TFramedTransport tframed;
        TProtocol protocol;
        MLtynHost.Client client ;
        public ThriftClient()
        {
            transport = new TSocket("192.168.9.203", 7911);
            tframed = new TFramedTransport(transport);
            protocol = new TCompactProtocol(tframed);
            client = new MLtynHost.Client(protocol);
        }
        public bool Operate(int slaveid,bool open)
        {
            try
            {
                if (!transport.IsOpen)
                {
                    transport.Open();
                }
                OperateError error = client.OperateDevice(slaveid, open);
                return error.Status;
            }
            catch(Exception ex)
            {
                LogHelper.Error("启停设备失败", ex);
                return false;
            }
        }
        public bool SetMode(int slaveid,int mode)
        {
            try
            {
                if (!transport.IsOpen)
                {
                    transport.Open();
                }
                OperateError error = client.SetMode(slaveid, mode);
                return error.Status;
            }
            catch (Exception ex)
            {
                LogHelper.Error("设置模式失败", ex);
                return false;
            }
        }
        public bool SetTemp(int slaveid,double temp)
        {
            try
            {
                if (!transport.IsOpen)
                {
                    transport.Open();
                }
                OperateError error = client.SetTemp(slaveid, temp);
                return error.Status;
            }
            catch (Exception ex)
            {
                LogHelper.Error("设置温度失败", ex);
                return false;
            }
        }
        public bool Refresh(int slaveid,bool operate)
        {
            try
            {
                if (!transport.IsOpen)
                {
                    transport.Open();
                }
                OperateError error = client.DeviceRefresh(slaveid,operate);
                return error.Status;
            }
            catch (Exception ex)
            {
                LogHelper.Error("刷新设备列表失败", ex);
                return false;
            }
        }
    
    }
}