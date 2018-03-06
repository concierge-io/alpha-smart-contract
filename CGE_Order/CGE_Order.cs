using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;

namespace Neo.SmartContract
{
    public class CGEOrder : Framework.SmartContract
    {
        public static readonly byte[] Owner = "Aap6bQ2Bmy1z64fjw645yUghQJRSNJkXbA".ToScriptHash();  // Should be change when deploy to MainNet

        public static Object Main(string operation, params object[] args)
        {
            if (Runtime.Trigger == TriggerType.Verification)
            {
                if (Owner.Length == 20)
                {
                    // if param Owner is script hash
                    return Runtime.CheckWitness(Owner);
                }
                else if (Owner.Length == 33)
                {
                    // if param Owner is public key
                    byte[] signature = operation.AsByteArray();
                   
                    return VerifySignature(signature, Owner);
                }
            }
            else if (Runtime.Trigger == TriggerType.Application)
            {
                if (!Runtime.CheckWitness(Owner))
                {
                    return false;
                }

                switch (operation)
                {
                    case "query":
                        return QueryOrder((string)args[0]);
                    case "create":
                        return CreateOrder((string)args[0], (string)args[1]);
                    case "createMultiple":
                        return CreateOrderMultiple(args);
                    case "update":
                        return UpdateOrder((string)args[0], (string)args[1]);
                    default:
                        return false;
                }
            }
           
            return false;
        }

        private static string TriggerString(TriggerType trigger)
        {
            if (trigger == TriggerType.Application)
            {
                return "Application";
            }
            else if (trigger == TriggerType.ApplicationR)
            {
                return "ApplicationR";
            }
            else if (trigger == TriggerType.Verification)
            {
                return "Verification";
            }
            else if (trigger == TriggerType.VerificationR)
            { 
                    return "VerificationR";
            }

            return "N/A";
        }
        /**
         * Store multiple order data to Storage
         */
        private static object CreateOrderMultiple(object[] args)
        {
            int len = args.Length;
            if (len % 2 != 0)
            {
                return false;
            }

            for (int i = 0; i < len; i = i + 2)
            {
                string key = (string)args[i];
                string value = (string)args[i + 1];
                Runtime.Log("key " + key);
                Runtime.Log("val " + value);

                Storage.Put(Storage.CurrentContext, key, value);
            }

            return true;
        }

        /**
        * Update order data in Storage
        */
        private static object UpdateOrder(string domain, string value)
        {
            byte[] v = Storage.Get(Storage.CurrentContext, domain);
            if (v == null) return false;

            Storage.Delete(Storage.CurrentContext, domain);
            Storage.Put(Storage.CurrentContext, domain, value);
            return true;
        }

        /**
        * Store order data to Storage
        */
        private static object CreateOrder(string domain, string value)
        {
            byte[] v = Storage.Get(Storage.CurrentContext, domain);
            if (v != null) return false;

            Storage.Put(Storage.CurrentContext, domain, value);
            return true;
        }

        /**
        * Query order data from Storage
        */
        private static object QueryOrder(string domain)
        {
            byte[] v = Storage.Get(Storage.CurrentContext, domain);
            if (v == null) return false;

            return v;
        }
    }
}

