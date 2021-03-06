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
public partial class WaterMeterPara : TBase
{
  private int _slaveID;
  private string _param;
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

  public string Param
  {
    get
    {
      return _param;
    }
    set
    {
      __isset.param = true;
      this._param = value;
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
    public bool param;
    public bool error;
  }

  public WaterMeterPara() {
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
          if (field.Type == TType.String) {
            Param = iprot.ReadString();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 3:
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
    TStruct struc = new TStruct("WaterMeterPara");
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
    if (Param != null && __isset.param) {
      field.Name = "param";
      field.Type = TType.String;
      field.ID = 2;
      oprot.WriteFieldBegin(field);
      oprot.WriteString(Param);
      oprot.WriteFieldEnd();
    }
    if (Error != null && __isset.error) {
      field.Name = "error";
      field.Type = TType.Struct;
      field.ID = 3;
      oprot.WriteFieldBegin(field);
      Error.Write(oprot);
      oprot.WriteFieldEnd();
    }
    oprot.WriteFieldStop();
    oprot.WriteStructEnd();
  }

  public override string ToString() {
    StringBuilder sb = new StringBuilder("WaterMeterPara(");
    sb.Append("SlaveID: ");
    sb.Append(SlaveID);
    sb.Append(",Param: ");
    sb.Append(Param);
    sb.Append(",Error: ");
    sb.Append(Error== null ? "<null>" : Error.ToString());
    sb.Append(")");
    return sb.ToString();
  }

}

