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

.. _rpcactors:

.. enum:: public enum RpcActors
    :values: None Host NonHost LastSender Everyone
    :val(1): Ignores sending / receiving of RPC
    :val(2): Only allows host to send / receive RPC
    :val(3): Allows any non-host to send / receive RPC
    :val(4): Receiver only! Special value which allows for senders to "respond" back to the last client that sent the specific RPC
    :val(5): Allows any caller to send / receive RPC

    Used in ModRPC attribute to specify allowed senders / receivers of RPC

.. end-type::

**Usage**

.. code-block:: csharp
    
    public enum MyRPCs {
        PublicSendMsg,
        HostSendMsg
    }

    // Sends / receives a message
    [ModRPC((uint)MyRPCs.PublicSendMessage)]
    public void AnyoneSendMessage(string message) {
        VentLogger.Info($"Message Received: {message}");
    }

    // Allows only host to send a message, and allows for only non-hosts to receive the message
    [ModRPC((uint)MyRPCs.HostSendMsg, senders: RpcActors.Host, receivers: RpcActors.NonHost]
    public void HostMessage(string message) {
        VentLogger.Info($"I am not the host and I received this: \"{message}\" message.);
    }

Interfaces
^^^^^^^^^^^^^^^^^

.. namespace:: VentLib.RPC.Interfaces

.. note:: You must declare a default, no-parameters constructor in implementing classes. Additionally, when declaring this interface on an abstract class it is required to register that class via the :type:`AbstractConstructors` class.

.. type:: public interface IRpcSendable<T> : IRpcReadable<T>, IRpcWritable

    When implemented on a type, allows for that type to be transfered and receieved via :type:`ModRPCAttribute` methods.

.. method:: public T Read(MessageReader reader)
    :param(1): The current message reader to pull data from.
    :returns: Newly constructed instance of class.

    This method is automatically called when receiving an RPC with T as a declared parameter. The ``MessageReader`` is automatically
    passed in and should be used to retrieve the necessary data in order to construct the object
    

.. method:: public void Write(MessageWriter writer)
    :param(1): The message writer, used to write current data about this instance.

    This method is automatically called when sending an RPC that declares the implementing type as a parameter. The ``MessageWriter`` is automatically
    passed, and should be used to write the information needed by :meth:`Read` to re-construct this object

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
    
    
Utilities
^^^^^^^^^^^^^^

.. namespace:: VentLib.RPC

.. type:: public class ModRPC
    
    The object representation for a method marked with ModRPC. Allows for single-use and targeted invocation of the related ModRPCAttribute method.

.. method:: public void Send(int[] clientIds, params object[] args)
    :param(1): An array of clientIds to specifically target with an RPC or null to target all clients
    :param(2): A variable number of arguments which matches the targeted method's signature.

    Sends a Custom RPC to the targeted client(s) with the passed in arguments.

.. method:: public void InvokeTrampoline(object[] args)
    :param(1): An array of objects representing the arguments of the original targeted method.
    
    Invokes the original, underlying, method with the given parameters without sending any Custom RPC.

.. end-type::

**Usage**

.. code-block:: csharp
    
    [ModRPC(0)]
    public void MyMethod(int a) {
        // Do something
    }

    public void ManualSendAndInvoke() {
        ModRPC myMethodModRPC; // Acquired usually through Vents.Find()
        // Below assumes myMethodModRPC is a valid object and not null.
        myMethodModRPC.Send(new int[] { 1 }, 3); // Sends Custom RPC 0 (defined by MyMethod) to client with the ID of 1
        
        myMethodModRPC.InvokeTrampoline(new object[] { 1 }); // Invokes "MyMethod(1)"
    }

.. seealso:: Refer to Vents.FindRPC() for acquiring a ModRPC instance


Example text with reference on `RpcActors <#enum-VentLib.RPC.Attributes.ModRPCAttribute.RpcActors>` _.

Vents
---------------------------

The main class for VentFramework which contains a couple utility methods, initialization methods,
and contains management instances for other modules.

.. type:: public static class Vents

Variables
^^^^^^^^^^^^^^^^^^^^^

.. variable:: public static readonly uint[] BuiltinRPCs
    
    An array of internal RPC ids used by VentFramework. These ids cannot be used with ModRPCAttribute and require the special VentRPCAttribute instead.

.. variable:: public static VersionControl VersionControl

    Provides an instance of the VersionControl class used for Client2Host handshakes and general versioning

.. variable:: public static CommandRunner CommandRunner

    Provides and instance of the CommandRunner class used for intercepting chat commands 

Methods
^^^^^^^^^^^^^^^^^^^^^^^

.. important:: All assemblies that utilize VentFramework should call Vents.Initialize() at least once!

.. method:: public void Initialize()

    Initializes VentFramework

.. method:: public void Register(Assembly assembly, bool localize = true)
    :param(1): The assembly to register with VentLib
    :param(2): If the assembly should be localized

    This method should be automatically called on all assemblies, but is provided for edge cases where it isn't.

.. method:: public ModRPC FindRPC(uint callerId, MethodInfo targetMethod = null)
    :returns: The first matching ModRPC or null if no such ModRPC exists
    :param(1): The unique RPC id to search for
    :param(2): If provided, grabs the ModRPC associated with specific method, assuming that method also corresponds to the callerId.

    Finds the first ModRPC associated with a unique RPC id, or the speciifc ModRPC linked with the target method, if specified.

**Usage:**
    
.. code-block:: csharp

    ModRPC rpc = Vents.FindRPC((uint)RPCs.ExampleRPC);

.. method:: public PlayerControl GetLastSender(uint callerId)
    :returns: The last sender of the specified RPC or null if there hasn't been a sender
    :param(1): The unique RPC id
    
    Gets the last sender of a specified RPC

**Usage:**

.. code-block:: csharp
    
    PlayerControl myPlayer = Vents.GetLastSender((uint)RPCs.ExampleRPC);

.. method:: public void BlockClient(Assembly assembly, int clientId)
    :param(1): The assembly to block RPC reception from
    :param(2): The specific client to block

    Blocks reception of RPCs from a speciifc client for all ModRPCs in a specified assembly
    