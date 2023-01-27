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

.. method:: public void Test()

    Testing this method

Interfaces
^^^^^^^^^^^^^^^^^

.. namespace:: VentLib.RPC.Interfaces

.. type:: public interface IRpcSendable<T> : IRpcReadable<T>, IRpcWritable

    When implemented on a type, allows for that type to be transfered and receieved via :type:`ModRPCAttribute` methods.

.. variable:: T where T: IRpcSendable<T>

.. end-type::


Enums
^^^^^^^^^^^^^^^^

.. type:: public enum 


Example text with reference on :type:`ModRPCAttribute`.