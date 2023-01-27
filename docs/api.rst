.. default-domain:: sphinxsharp

API Reference
======================
The following section outlines the API of VentFramework

Custom RPC
---------------------
Attributes
^^^^^^^^^^^^^^^^^

.. namespace:: VentLib.RPC.Attributes

.. type:: public class ModRPCAttribute : Attribute

    The ModRPC attribute is the basis for Custom RPCs. This attribute can be put over any method (static or non-static),
    and it'll be automatically picked up by the framework for custom rpc use. 

.. important:: Non-static methods are only supported in classes implementing :type:`IRpcInstance`

The following parameters are supported by ModRPC:

* Any native type (bool, int, uint64, string, double, etc)
* Any type implementing the interface :type:`IRpcSendable`
* Any List<T> where T is any of the above type
* Additionally, optional parameters are allowed BUT ModRPC does not support ANY ``null`` values passed into the method.

.. method:: public ModRPCAttribute(uint rpc, RpcActors senders, RpcActors receivers, MethodInvocation invocation)
    :param(1): The custom and unique rpc-id to send.
    :param(2): **Default: RpcActors.Everyone** the allowed sender of this RPC. Note: calls to this method from non-allowed senders ONLY blocks the RPC from being sent, based on the :type:`MethodInvocation` parameter, this method still may end up running.
    :param(3): **Default: RpcActors.Everyone** the allowed receiver of this RPC. This rule is handled by the receiving client and NOT the sending client.
    :param(4): If and when the code for the method should be run.

.. method:: public void Test(string s)
    :param(1): String string

    Testing this method

Interfaces
^^^^^^^^^^^^^^^^^

.. namespace:: VentLib.RPC.Interfaces

.. note:: You must declare a default, no-parameters constructor in implementing classes. Additionally, when declaring this interface on an abstract class it is required to register that class via the :type:`AbstractConstructors` class.

.. type:: public interface IRpcSendable<T>

    When implemented on a type, allows for that type to be transfered and receieved via :type:`ModRPCAttribute` methods.

.. method:: public T Read(MessageReader reader)
    This method is automatically called when receiving an RPC with T as a declared parameter. The ``MessageReader`` is automatically
    passed in and should be used to retrieve the necessary data in order to construct the object
    :param(1): The current message reader to pull data from.
    :returns: Newly constructed instance of class.

.. method:: public void Write(MessageWriter writer)
    This method is automatically called when sending an RPC that declares the implementing type as a parameter. The ``MessageWriter`` is automatically
    passed, and should be used to write the information needed by :meth:`Read` to re-construct this object
    :param(1): The message writer, used to write current data about this instance.

.. end-type::



**Usage**

.. code-block:: csharp
    
    public class MyObject : IRpcSendable<MyObject> {
        public int a;
        
        public MyObject(int a) {
            this.a = a;
        }
        
        public MyObject Read(MessageReader reader) {
            return new MyObject(reader.ReadInt32()); // read integer value from reader and construct new object from it
        }

        public void Write(MessageWriter writer) {
            write.Write(this.a); // write this object's value to the message writer
        }
    }




Enums
^^^^^^^^^^^^^^^^

.. type:: public enum 


Example text with reference on :type:`ModRPCAttribute`.