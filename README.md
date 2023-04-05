### **Version 1.0.0**
The first official release of VentLib/VentFramework. These features are still experimental but this release marks the turning point where I feel the framework has enough features that should be stable enough for public use. As traffic comes in and people begin using the framework I will maintain and fix all issues.
### **Features:**

- Highly robust option system aiming to replace BepIn's builtin configuration
    - Out-of-the-box integration with Among Us' game options
    - Highly configurable rendering for the game options
- Brand new localization system that emphasizes easy usability and organization
    - Translations are obtainable statically via attributes or a static method call
- Easy command manager for Among Us chat commands
    - Decorate classes with the command prefix then allow the framework to do all the heavy-lifting
- Super configurable modded handshake support
    - Configure the behavior when modded clients join (Kick, Disable RPC, do nothing)
- Modded "Find A Game" menu which shows modded lobbies with a cleaner UI
- Custom RPC attributes with cleaner developmental interface than Reactor
    - Batch interface which allows for objects to be sent across multiple packets
- Loads of new utilities
    - New data structures
        - Batch collection (for automatic sending list of objects with Batch-RPC)
        - An array of "Optional" objects highly influenced by Java Optional API
        - A whole bunch more
   - Revamped asynchronous methods for running & scheduling code asynchronously
   - Profiling utilities 

This mod is not affiliated with Among Us or Innersloth LLC, and the content contained therein is not endorsed or otherwise sponsored by Innersloth LLC. Portions of the materials contained herein are property of Innersloth LLC. © Innersloth LLC.
