/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright by The HDF Group.                                               *
 * Copyright by the Board of Trustees of the University of Illinois.         *
 * All rights reserved.                                                      *
 *                                                                           *
 * This file is part of HDF.PInvoke. Copyright notices and redistribution
 * terms for the wrapper and its native dependencies are in LICENSE.txt
 * at the repository and package root.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using HDF.PInvoke;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using hid_t = System.Int64;
using hsize_t = System.UInt64;

namespace UnitTests
{
    /// <summary>
    /// Groups regression checks for the H5T API.
    /// </summary>
    public partial class H5TTest
    {
        /// <summary>
        /// Exercises the native <c>H5Tinsert</c> operation in scenario 1.
        /// </summary>
        [TestMethod]
        public void H5TinsertTest1()
        {
            // a fixed-length string type
            var fls = H5T.create(H5T.class_t.STRING, new IntPtr(16));
            Assert.IsTrue(fls >= 0);
            Assert.IsTrue(H5T.is_variable_str(fls) == 0);

            // a variable-length string type
            var vls = H5T.create(H5T.class_t.STRING, H5T.VARIABLE);
            Assert.IsTrue(vls >= 0);
            Assert.IsTrue(H5T.is_variable_str(vls) > 0);

            // a key-value compound
            var size = new IntPtr(16 + IntPtr.Size);
            var kvt = H5T.create(H5T.class_t.COMPOUND, size);
            Assert.IsTrue(H5T.insert(kvt, "key", IntPtr.Zero, fls) >= 0);
            Assert.IsTrue(H5T.insert(kvt, "value", new IntPtr(16), vls) >= 0);
            Assert.IsTrue(H5T.close(vls) >= 0);
            Assert.IsTrue(H5T.close(fls) >= 0);

            // create a key-value dataset (3 elements)

            var fsp = H5S.create_simple(1, new hsize_t[] { 3 }, null);
            Assert.IsTrue(fsp >= 0);

            var dset = H5D.create(m_v2_class_file, "KeyVal", kvt, fsp);
            Assert.IsTrue(dset >= 0);
            Assert.IsTrue(H5S.close(fsp) >= 0);

            // write a 3 elements

            var keys = new string[] {
                "Key0123456789ABC", "Key0123456789DEF", "Key0123456789GHI"};

            var values = new IntPtr[3];
            values[0] = Marshal.StringToHGlobalAnsi("I am a managed String!");
            values[1] = Marshal.StringToHGlobalAnsi("I am also a managed String!");
            values[2] = Marshal.StringToHGlobalAnsi("I am another managed String!");

            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);

            for (var i = 0; i < 3; ++i)
            {
                writer.Write(Encoding.ASCII.GetBytes(keys[i]));

                if (IntPtr.Size == 8)
                {
                    writer.Write(values[i].ToInt64());
                }
                else
                {
                    writer.Write(values[i].ToInt32());
                }
            }

            var wdata = ms.ToArray();
            var hnd = GCHandle.Alloc(wdata, GCHandleType.Pinned);

            Assert.IsTrue(H5D.write(dset, kvt, H5S.ALL, H5S.ALL, H5P.DEFAULT,
                hnd.AddrOfPinnedObject()) >= 0);

            hnd.Free();

            // now read it back

            var rdata = new byte[3 * size.ToInt32()];
            hnd = GCHandle.Alloc(rdata, GCHandleType.Pinned);

            Assert.IsTrue(H5D.read(dset, kvt, H5S.ALL, H5S.ALL, H5P.DEFAULT,
                 hnd.AddrOfPinnedObject()) >= 0);

            hnd.Free();

            // check it out

            var ms1 = new MemoryStream(rdata);
            var reader = new BinaryReader(ms1);

            for (var i = 0; i < 3; ++i)
            {
                var k = Encoding.ASCII.GetString(reader.ReadBytes(16));
                Assert.IsTrue(k == keys[i]);
                var ptr = IntPtr.Zero;

                if (IntPtr.Size == 8)
                {
                    ptr = new IntPtr(reader.ReadInt64());
                }
                else
                {
                    ptr = new IntPtr(reader.ReadInt32());
                }

                var v = Marshal.PtrToStringAnsi(ptr);
                Assert.IsTrue(v == Marshal.PtrToStringAnsi(values[i]));
                Marshal.FreeHGlobal(ptr);
                Marshal.FreeHGlobal(values[i]);
            }

            Assert.IsTrue(H5D.close(dset) >= 0);
            Assert.IsTrue(H5T.close(kvt) >= 0);
        }
    }
}
