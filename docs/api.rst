.. default-domain:: sphinxsharp

API Reference
======================
The following section outlines the API of VentFramework

Custom RPC
---------------------
Attributes
^^^^^^^^^^^^^^^^^

.. namespace:: VentFramework.RPC.Attributes

.. type:: public class ModRPCAttribute: System.Attribute

    .. method:: public ModRPCAttribute(uint rpc, RpcActors senders = RpcActors.Everyone, RpcActors receivers = RpcActors.Everyone, MethodInvocation invocation = MethodInvocation.ExecuteNever) 

Example text with reference on :type:`ModRPCAttribute`.