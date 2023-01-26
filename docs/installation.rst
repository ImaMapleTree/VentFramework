Installation
========================

This page covers two different types of installations.
If one of your mods requires VentFramework and you need the to install it into your Among Us folder refer to section below. Otherwise,
if you're trying to use VentFramework as a package reference refer to :ref:`install-package-ref`


Installing Locally
----------------------

**Prerequisites**
^^^^^^^^^^^^^^^^^^^^^

Make sure these are downloaded and installed in your local directory before continuing.

* `Among Us <https://www.innersloth.com/games/among-us/>`_ - Base game
* `BepInEx <https://github.com/BepInEx/BepInEx/>`_ - Required for C# code injection

To install you'll need to download VentFramework. Mod authors should provide which version of the framework you'll need.
But versions are usually cross-compatible so if confused try downloading the latest.

**All versions of VentFramework can be found** `here <https://www.nuget.org/packages/VentFramework/>`_

Once downloaded you should have a file named something similar to ``ventframework.A.B.C.D.nupkg`` Unzip that file, open it, and navigae to the directory ``lib/net6.0/``
where you should find the **VentFramework.dll**

All that's left is to move that file to your Among Us directory, into the ``BepinEx/Plugins`` folder. Once it's there you're good to go!

.. _install-package-ref:

Installing As Package Reference
----------------------------------

This installation is for if you're a developer and your code relies on VentFramework. 

Installing is simply a matter of updating your .csproj to include VentFramework as a reference
.. code-block:: xml
    <PackageReference Condition="$(Debug) == 'false'" Include="VentFramework" Version="1.0.7.69"/>
