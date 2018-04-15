/**
 * Autogenerated by Thrift Compiler (0.9.1)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;
using System.Runtime.Serialization;
using Thrift.Protocol;
using Thrift.Transport;


#if !SILVERLIGHT
[Serializable]
#endif
public partial class HostPara : TBase
{
  private int _slaveID;
  private int _mode;
  private string _tmp;
  private int _runStatus;
  private string _msg;
  private OperateError _error;

  public int SlaveID
  {
    get
    {
      return _slaveID;
    }
    set
    {
      __isset.slaveID = true;
      this._slaveID = value;
    }
  }

  public int Mode
  {
    get
    {
      return _mode;
    }
    set
    {
      __isset.mode = true;
      this._mode = value;
    }
  }

  public string Tmp
  {
    get
    {
      return _tmp;
    }
    set
    {
      __isset.tmp = true;
      this._tmp = value;
    }
  }

  public int RunStatus
  {
    get
    {
      return _runStatus;
    }
    set
    {
      __isset.runStatus = true;
      this._runStatus = value;
    }
  }

  public string Msg
  {
    get
    {
      return _msg;
    }
    set
    {
      __isset.msg = true;
      this._msg = value;
    }
  }

  public OperateError Error
  {
    get
    {
      return _error;
    }
    set
    {
      __isset.error = true;
      this._error = value;
    }
  }


  public Isset __isset;
  #if !SILVERLIGHT
  [Serializable]
  #endif
  public struct Isset {
    public bool slaveID;
    public bool mode;
    public bool tmp;
    public bool runStatus;
    public bool msg;
    public bool error;
  }

  public HostPara() {
  }

  public void Read (TProtocol iprot)
  {
    TField field;
    iprot.ReadStructBegin();
    while (true)
    {
      field = iprot.ReadFieldBegin();
      if (field.Type == TType.Stop) { 
        break;
      }
      switch (field.ID)
      {
        case 1:
          if (field.Type == TType.I32) {
            SlaveID = iprot.ReadI32();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 2:
          if (field.Type == TType.I32) {
            Mode = iprot.ReadI32();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 3:
          if (field.Type == TType.String) {
            Tmp = iprot.ReadString();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 4:
          if (field.Type == TType.I32) {
            RunStatus = iprot.ReadI32();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 5:
          if (field.Type == TType.String) {
            Msg = iprot.ReadString();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 6:
          if (field.Type == TType.Struct) {
            Error = new OperateError();
            Error.Read(iprot);
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        default: 
          TProtocolUtil.Skip(iprot, field.Type);
          break;
      }
      iprot.ReadFieldEnd();
    }
    iprot.ReadStructEnd();
  }

  public void Write(TProtocol oprot) {
    TStruct struc = new TStruct("HostPara");
    oprot.WriteStructBegin(struc);
    TField field = new TField();
    if (__isset.slaveID) {
      field.Name = "slaveID";
      field.Type = TType.I32;
      field.ID = 1;
      oprot.WriteFieldBegin(field);
      oprot.WriteI32(SlaveID);
      oprot.WriteFieldEnd();
    }
    if (__isset.mode) {
      field.Name = "mode";
      field.Type = TType.I32;
      field.ID = 2;
      oprot.WriteFieldBegin(field);
      oprot.WriteI32(Mode);
      oprot.WriteFieldEnd();
    }
    if (Tmp != null && __isset.tmp) {
      field.Name = "tmp";
      field.Type = TType.String;
      field.ID = 3;
      oprot.WriteFieldBegin(field);
      oprot.WriteString(Tmp);
      oprot.WriteFieldEnd();
    }
    if (__isset.runStatus) {
      field.Name = "runStatus";
      field.Type = TType.I32;
      field.ID = 4;
      oprot.WriteFieldBegin(field);
      oprot.WriteI32(RunStatus);
      oprot.WriteFieldEnd();
    }
    if (Msg != null && __isset.msg) {
      field.Name = "msg";
      field.Type = TType.String;
      field.ID = 5;
      oprot.WriteFieldBegin(field);
      oprot.WriteString(Msg);
      oprot.WriteFieldEnd();
    }
    if (Error != null && __isset.error) {
      field.Name = "error";
      field.Type = TType.Struct;
      field.ID = 6;
      oprot.WriteFieldBegin(field);
      Error.Write(oprot);
      oprot.WriteFieldEnd();
    }
    oprot.WriteFieldStop();
    oprot.WriteStructEnd();
  }

  public override string ToString() {
    StringBuilder sb = new StringBuilder("HostPara(");
    sb.Append("SlaveID: ");
    sb.Append(SlaveID);
    sb.Append(",Mode: ");
    sb.Append(Mode);
    sb.Append(",Tmp: ");
    sb.Append(Tmp);
    sb.Append(",RunStatus: ");
    sb.Append(RunStatus);
    sb.Append(",Msg: ");
    sb.Append(Msg);
    sb.Append(",Error: ");
    sb.Append(Error== null ? "<null>" : Error.ToString());
    sb.Append(")");
    return sb.ToString();
  }

}
