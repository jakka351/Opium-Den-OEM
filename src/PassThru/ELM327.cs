# ////////////////////////////////////////////////////////////////////////////////////////////////////
#                            Tester Present Specialist Automotive Solutions                         //
# ////////////////////////////////////////////////////////////////////////////////////////////////////                                                                                 
#
#                             ___________              __                 __________                                      __                         
#                             \__    ___/___   _______/  |_  ___________  \______   \_______   ____   ______ ____   _____/  |_                       
#                               |    |_/ __ \ /  ___/\   __\/ __ \_  __ \  |     ___/\_  __ \_/ __ \ /  ___// __ \ /    \   __\                      
#                               |    |\  ___/ \___ \  |  | \  ___/|  | \/  |    |     |  | \/\  ___/ \___ \\  ___/|   |  \  |                        
#                               |____| \___  >____  > |__|  \___  >__|     |____|     |__|    \___  >____  >\___  >___|  /__|                        
#                                          \/     \/            \/                                \/     \/     \/     \/                            
#                               _________                    .__       .__  .__          __                                                          
#                              /   _____/_____   ____   ____ |__|____  |  | |__| _______/  |_                                                        
#                              \_____  \\____ \_/ __ \_/ ___\|  \__  \ |  | |  |/  ___/\   __\                                                       
#                              /        \  |_> >  ___/\  \___|  |/ __ \|  |_|  |\___ \  |  |                                                         
#                             /_______  /   __/ \___  >\___  >__(____  /____/__/____  > |__|                                                         
#                                     \/|__|        \/     \/        \/             \/                                                               
#                                _____          __                         __  .__                                                                   
#                               /  _  \  __ ___/  |_  ____   _____   _____/  |_|__|__  __ ____                                                       
#                              /  /_\  \|  |  \   __\/  _ \ /     \ /  _ \   __\  \  \/ // __ \                                                      
#                             /    |    \  |  /|  | (  <_> )  Y Y  (  <_> )  | |  |\   /\  ___/                                                      
#                             \____|__  /____/ |__|  \____/|__|_|  /\____/|__| |__| \_/  \___  >                                                     
#                                     \/                         \/                          \/                                                      
#                               _________      .__          __  .__                                                                                  
#                              /   _____/ ____ |  |  __ ___/  |_|__| ____   ____   ______                                                            
#                              \_____  \ /  _ \|  | |  |  \   __\  |/  _ \ /    \ /  ___/                                                            
#                              /        (  <_> )  |_|  |  /|  | |  (  <_> )   |  \\___ \                                                              
#                             /_______  /\____/|____/____/ |__| |__|\____/|___|  /____  >                                                           
#                                     \/                                       \/     \/                                                             
#
#
#
#                              Tester Present Specialist Automotive Solutions Open Source License (TPSAS-OSL)
#                              Version 1.0, 2026
#
#                              Copyright (c) 2026 Tester Present Specialist Automotive Solutions
#
#
#             Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
#             documentation files (the "Software"), to use, modify, and distribute the Software subject to the following conditions:
#
#          1. Attribution.
#            - The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
#            - Credit must be given to "Tester Present Specialist Automotive Solutions" in any documentation, public notices, or product materials 
#              that acknowledge the use of the Software.//
#
#          2. Open Source Use Only.
#            - The Software, including any modifications or derivative works (collectively, “Derivatives”), must be distributed only as open source software.
#            - Derivatives or distributions of the Software must be licensed under this license or another license approved by the Open Source Initiative 
#              (OSI) that maintains equivalent open source freedoms.
#            - The Software and any Derivatives may not be used as a component in any closed source or proprietary software product.
#
#          3. Redistribution.
#            - Redistributions of the Software, in source or binary form, must reproduce the above attribution, this license, and the disclaimer below 
#              in the documentation and/or other materials provided with the distribution.
#            - Any modifications made to the Software must be clearly marked as such, and the modified files must carry prominent notices stating that
#              changes have been made.
#
#          4. No Warranty.
#            - THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#              FITNESS FOR A PARTICULAR PURPOSE, AND NONINFRINGEMENT.
#            - IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES, OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
#              TORT, OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#
#          5. Termination.
#            - This license and the rights granted hereunder will terminate automatically upon any breach of the above conditions.
#            - Upon termination, you must cease all use and distribution of the Software and any Derivatives.
#
#          6. Entire Agreement.
#            - This license constitutes the entire agreement between you and Tester Present Specialist Automotive Solutions regarding the Software 
#              and supersedes any prior understandings or agreements, whether written or oral, regarding the subject matter herein.
#
#              By using, modifying, or distributing the Software, you agree to abide by all the terms and conditions of this license.
#
#
#                         

// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Tester Present Specialist Automotive Solutions - ELM327 Serial Transport Shim
//  Copyright (c) 2026 Benjamin Jack Leighton. All rights reserved.
//
//  Provides a J2534-compatible communication layer over ELM327 serial adapters.
//  Translates J2534 API semantics (SendReceive, filters, protocols) into ELM327 AT commands.
//  Thread-safe — all serial port access is serialised via _portLock.
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace J2534
{
    /// <summary>
    /// Low-level serial transport for ELM327 OBD-II adapters.
    /// Maps J2534-style SendReceive calls to ELM327 AT command sequences.
    /// </summary>
    public class ELM327Transport : IDisposable
    {
        // ---- Serial port ----
        private SerialPort _port;
        private readonly object _portLock = new object();
        private bool _isOpen;
        private string _comPort;

        // ---- ELM327 state ----
        private string _elmVersion = "";
        private string _elmChipId = "";
        private ProtocolID _currentProtocol = ProtocolID.ISO15765;
        private uint _currentTxId;
        private uint _currentRxId;
        private bool _is29Bit;   // true after ATSP7 — drives SendReceive29Bit's restore of 11-bit mode
        private bool _disposed;

        // ---- TesterPresent software timer ----
        private Timer _testerPresentTimer;
        private uint _tpTxId;
        private volatile bool _tpActive;

        // ---- Events (wired from J2534Manager) ----
        public event Action<string> OnLog;
        public event Action<string> OnError;

        // ---- OBDLink / STN (ST command set) ----
        // STN-based OBDLink devices (STN11xx/STN21xx) are a superset of ELM327: on top of the AT set
        // they expose the ST command set, which unlocks capabilities a generic ELM327 cannot reach —
        // Ford MS-CAN (pins 3/11, 125k via STP 53/54), raw-CAN monitoring + filtering (STM/STMA),
        // SW-CAN/GMLAN, J1939, selectable baud (STPBR) and flexible requests (STPX). Detected at
        // Initialize() via the STI identification command; generic clones leave _isStn=false.
        private bool _isStn;
        private string _stnDevice = "";    // e.g. "STN1170" / "OBDLink MX+"
        private string _stnVersion = "";   // STN firmware version string
        // VLinker FS (Vgate) is an STN-core device with a proprietary VT* layer. For diagnostics it IS an
        // OBDLink/STN (handled by the STN path); VTI just identifies it for a nicer label. The VT* flash
        // commands are deliberately NOT used — flashing stays J2534-only.
        private bool _isVLinker;
        private string _vLinkerVersion = "";
        // Raw-CAN monitor (sniffer) state — STMA streams frames continuously until interrupted.
        private bool _monitorActive;
        private double _lastBatteryVolts;   // last good ATRV reading; returned while the STMA monitor is streaming
        private readonly StringBuilder _monBuf = new StringBuilder();

        // ---- Public state ----
        public bool IsOpen => _isOpen;
        public string ElmVersion => _elmVersion;
        public string ElmChipId => _elmChipId;
        public string ComPort => _comPort;
        /// <summary>True when the connected adapter is an OBDLink/STN device (ST command set available).</summary>
        public bool IsStn => _isStn;
        /// <summary>STN device description (e.g. "OBDLink MX+") or "" on a generic ELM327.</summary>
        public string StnDevice => _stnDevice;
        /// <summary>STN firmware version string or "" on a generic ELM327.</summary>
        public string StnVersion => _stnVersion;
        /// <summary>True when the adapter identifies as a VLinker FS (Vgate STN-core device).</summary>
        public bool IsVLinker => _isVLinker;
        /// <summary>VLinker firmware string (from VTI), or "".</summary>
        public string VLinkerVersion => _vLinkerVersion;

        // ---- ELM327 protocol numbers ----
        private const int ELM_PROTO_AUTO      = 0;
        private const int ELM_PROTO_J1850_PWM = 1;
        private const int ELM_PROTO_J1850_VPW = 2;
        private const int ELM_PROTO_ISO9141   = 3;
        private const int ELM_PROTO_KWP2000_SLOW = 4;
        private const int ELM_PROTO_KWP2000_FAST = 5;
        private const int ELM_PROTO_CAN_11_500 = 6;
        private const int ELM_PROTO_CAN_29_500 = 7;
        private const int ELM_PROTO_CAN_11_250 = 8;
        private const int ELM_PROTO_CAN_29_250 = 9;

        // ---- Serial baud rates to try ----
        private static readonly int[] BAUD_RATES = { 115200, 38400, 9600, 57600, 230400 };

        #region Connection

        /// <summary>
        /// Open serial port and attempt to detect ELM327 adapter.
        /// Tries multiple baud rates if the specified one fails.
        /// </summary>
        public bool Open(string comPort, int preferredBaud = 115200)
        {
            lock (_portLock)
            {
                if (_isOpen) Close();
                _comPort = comPort;

                // Try preferred baud first, then fallbacks
                var baudsToTry = new List<int> { preferredBaud };
                foreach (int b in BAUD_RATES)
                    if (b != preferredBaud) baudsToTry.Add(b);

                foreach (int baud in baudsToTry)
                {
                    try
                    {
                        _port = new SerialPort(comPort, baud, Parity.None, 8, StopBits.One)
                        {
                            ReadTimeout = 3000,
                            WriteTimeout = 2000,
                            DtrEnable = true,
                            RtsEnable = true,
                            NewLine = "\r",
                            Encoding = Encoding.ASCII
                        };
                        _port.Open();

                        // Drain any garbage
                        Thread.Sleep(100);
                        _port.DiscardInBuffer();
                        _port.DiscardOutBuffer();

                        // Try ATZ reset
                        string response = SendCommandRaw("ATZ", 3000);
                        if (response != null && response.Contains("ELM"))
                        {
                            // Extract version from ATZ response (e.g. "ELM327 v2.1")
                            _elmVersion = ExtractElmVersion(response);
                            _isOpen = true;
                            RaiseLog($"ELM327 detected on {comPort} at {baud} baud: {_elmVersion}");
                            return true;
                        }

                        // Not ELM327 at this baud rate
                        _port.Close();
                        _port.Dispose();
                        _port = null;
                    }
                    catch
                    {
                        try { _port?.Close(); } catch { }
                        try { _port?.Dispose(); } catch { }
                        _port = null;
                    }
                }

                RaiseError($"No ELM327 adapter detected on {comPort}");
                return false;
            }
        }

        /// <summary>Close the serial port and release resources.</summary>
        public void Close()
        {
            lock (_portLock)
            {
                StopTesterPresent();
                _isOpen = false;
                _elmVersion = "";
                _elmChipId = "";

                if (_port != null)
                {
                    try
                    {
                        if (_port.IsOpen)
                        {
                            // Reset ELM327 before closing
                            try { _port.Write("ATZ\r"); } catch { }
                            Thread.Sleep(100);
                            _port.Close();
                        }
                    }
                    catch { }
                    try { _port.Dispose(); } catch { }
                    _port = null;
                }
            }
        }

        /// <summary>
        /// Send the standard initialization sequence to configure ELM327
        /// for raw hex communication with headers visible.
        /// </summary>
        public bool Initialize()
        {
            lock (_portLock)
            {
                if (!_isOpen) return false;

                try
                {
                    // Echo OFF — prevents command echo in responses
                    SendCommandChecked("ATE0");

                    // Linefeeds OFF — cleaner response parsing
                    SendCommandChecked("ATL0");

                    // Spaces OFF — compact hex output (e.g. "7E80362" instead of "7E8 03 62")
                    SendCommandChecked("ATS0");

                    // Headers ON — include CAN ID / address bytes in response
                    SendCommandChecked("ATH1");

                    // CAN Auto-Formatting ON — ELM327 handles ISO-TP framing
                    SendCommandChecked("ATCAF1");

                    // Set adaptive timeout — 0x64 = 100 * 4.096ms ≈ 400ms per response wait.
                    // Provides enough margin for slow ECUs while being responsive for polling/logging.
                    // (Previously ATSTFF = 1020ms which caused severe delays on unsupported PIDs)
                    SendCommandChecked("ATST64");

                    // Enable adaptive timing (level 2 = aggressive) — ELM327 learns ECU response
                    // speed and shortens wait time automatically for supported PIDs.
                    SendCommandChecked("ATAT2");

                    // Allow long messages (>7 bytes, multi-frame ISO-TP)
                    SendCommandChecked("ATAL");

                    // Try to read chip identifier (optional, may not be supported on clones)
                    string chipResponse = SendCommandRaw("AT@1", 1000);
                    if (chipResponse != null && !chipResponse.Contains("?"))
                        _elmChipId = chipResponse.Replace("OK", "").Trim();

                    RaiseLog("ELM327 initialized: echo=off, headers=on, spaces=off, CAF=on, timeout=400ms, adaptive=on");

                    // OBDLink / STN detection. STI returns the STN firmware id (e.g. "STN1170 v5.5.5");
                    // a generic ELM327 clone returns "?" or nothing. When present, the richer ST command
                    // set is available (MS-CAN, raw-CAN monitoring, STPX) — see IsStn consumers.
                    DetectStn();

                    return true;
                }
                catch (Exception ex)
                {
                    RaiseError("ELM327 initialization failed: " + ex.Message);
                    return false;
                }
            }
        }

        #endregion

        #region Protocol Selection

        /// <summary>Set CAN ISO 15765 protocol with 11-bit IDs at 500kbaud.</summary>
        public bool SetProtocolCAN(uint txId, uint rxId)
        {
            lock (_portLock)
            {
                if (!_isOpen) return false;

                // Set protocol: CAN 11-bit 500k
                SendCommandChecked("ATSP6");
                _is29Bit = false;

                // Set transmit header (CAN arbitration ID)
                string txHex = txId.ToString("X3");
                SendCommandChecked("ATSH" + txHex);

                _currentProtocol = ProtocolID.ISO15765;
                _currentTxId = txId;

                // Set CAN receive address filter BEFORE the protocol-detect query so the
                // first 0100 broadcast doesn't accept responses from random ECUs on a busy
                // bus. (Previously the filter was applied AFTER the test query, which let
                // wrong-ECU data leak into the very first read on multi-ECU vehicles.)
                string rxHex = rxId.ToString("X3");
                SendCommandChecked("ATCRA" + rxHex);
                _currentRxId = rxId;

                // Lock the FC source ID to our TX header so multi-frame ECU responses don't
                // get rejected by strict-stack ECUs (some Mazda / Subaru variants check this).
                ConfigureFlowControlDefaults(txId);

                // Send a test query (Mode 01 PID 00 — supported PIDs) to force the ELM327
                // to actually establish CAN bus communication. Some clones won't detect the
                // protocol until real data is sent. Use max timeout for the first query since
                // the adapter may need to search for the protocol.
                SendCommandChecked("ATSTFF");           // Max timeout for first query
                string testResponse = SendCommandRaw("0100", 5000);
                SendCommandChecked("ATST64");           // Restore normal timeout

                if (testResponse != null && !IsErrorResponse(testResponse))
                    RaiseLog("ELM327 protocol established — CAN bus active");
                else
                    RaiseLog("ELM327 protocol set (no ECU response yet — bus may be inactive)");

                RaiseLog($"ELM327 protocol: CAN 11-bit 500k | TX: 0x{txHex} | RX: 0x{rxHex}");
                return true;
            }
        }

        /// <summary>Set CAN ISO 15765 protocol with 29-bit IDs at 500kbaud.</summary>
        public bool SetProtocolCAN29Bit(uint txId, uint rxId)
        {
            lock (_portLock)
            {
                if (!_isOpen) return false;

                SendCommandChecked("ATSP7");
                _is29Bit = true;

                string txHex = txId.ToString("X8");
                SendCommandChecked("ATSH" + txHex);

                string rxHex = rxId.ToString("X8");
                SendCommandChecked("ATCRA" + rxHex);

                _currentProtocol = ProtocolID.ISO15765;
                _currentTxId = txId;
                _currentRxId = rxId;

                RaiseLog($"ELM327 protocol: CAN 29-bit 500k | TX: 0x{txHex} | RX: 0x{rxHex}");
                return true;
            }
        }

        /// <summary>Probe for an OBDLink/STN device and record its id + firmware version. Safe on a
        /// generic ELM327 (leaves <see cref="IsStn"/> false). Called at the end of <see cref="Initialize"/>.</summary>
        private void DetectStn()
        {
            try
            {
                string sti = SendCommandRaw("STI", 1000);     // STN firmware id, e.g. "STN1170 v5.5.5"
                if (!string.IsNullOrEmpty(sti) && !IsErrorResponse(sti) &&
                    sti.IndexOf("STN", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    _isStn = true;
                    _stnVersion = CleanLine(sti);
                    string dev = SendCommandRaw("STDI", 800);  // device description, e.g. "OBDLink MX+"
                    if (!string.IsNullOrEmpty(dev) && !IsErrorResponse(dev)) _stnDevice = CleanLine(dev);
                    // VLinker FS identifies via the Vgate-proprietary VTI command. Informational only —
                    // all functionality already works through the STN path; we just label it nicely.
                    string vti = SendCommandRaw("VTI", 800);
                    if (!string.IsNullOrEmpty(vti) && !IsErrorResponse(vti))
                    {
                        _isVLinker = true;
                        _vLinkerVersion = CleanLine(vti);
                        if (_stnDevice.Length == 0) _stnDevice = "vLinker FS";
                    }
                    RaiseLog($"OBDLink/STN detected: {(_stnDevice.Length > 0 ? _stnDevice + " " : "")}{_stnVersion}"
                           + (_isVLinker ? $" [VLinker {_vLinkerVersion}]" : "")
                           + " — ST command set enabled (MS-CAN, raw-CAN monitor, STPX).");
                }
                else
                {
                    RaiseLog("Generic ELM327 (no STN) — AT set only; MS-CAN, SW-CAN and raw-CAN monitoring "
                           + "are unavailable on this adapter.");
                }
            }
            catch { /* best-effort — stay in generic-ELM327 mode on any error */ }
        }

        private static string CleanLine(string s)
            => (s ?? "").Replace("OK", "").Replace(">", "").Replace("\r", " ").Replace("\n", " ").Trim();

        /// <summary>
        /// Select a CAN ISO-TP protocol using the OBDLink STP presets (STN devices ONLY). This is how an
        /// OBDLink reaches buses a generic ELM327 AT adapter cannot — notably Ford MS-CAN (pins 3/11,
        /// 125 kbps) via preset 53/54. The STN has a single CAN peripheral mapped to the HS/MS/SW pins
        /// under software control, so selecting a preset also switches the physical bus. Returns false on
        /// a non-STN adapter so the caller falls back to the 11/29-bit AT path (HS-CAN only).
        /// </summary>
        public bool SetProtocolStnCan(J2534Manager.CanBusKind bus, uint txId, uint rxId, bool is29Bit, uint baud = 0)
        {
            lock (_portLock)
            {
                if (!_isOpen) return false;
                if (!_isStn)
                {
                    RaiseLog("SetProtocolStnCan: adapter is not an OBDLink/STN — MS-CAN/SW-CAN unavailable on a generic ELM327.");
                    return false;
                }

                int preset; string busLabel;
                switch (bus)
                {
                    case J2534Manager.CanBusKind.MsCan: preset = is29Bit ? 54 : 53; busLabel = "MS-CAN 125k (pins 3/11)"; break;
                    case J2534Manager.CanBusKind.SwCan: preset = is29Bit ? 64 : 63; busLabel = "SW-CAN 33.3k (pin 1)";   break;
                    default:                            preset = is29Bit ? 34 : 33; busLabel = "HS-CAN 500k (pins 6/14)"; break;
                }

                // STP selects the physical transceiver + ISO-TP framing (it does not open the channel;
                // the first request does). Returns '?' if the preset is unsupported on this device.
                string r = SendCommandRaw("STP" + preset, 1500);
                if (r == null || IsErrorResponse(r)) { RaiseError($"STP{preset} rejected — this adapter may not support {busLabel}."); return false; }
                _is29Bit = is29Bit;
                _currentProtocol = ProtocolID.ISO15765;

                if (baud != 0) SendCommandChecked("STPBR " + baud.ToString(CultureInfo.InvariantCulture));

                // Headers + receive filter + flow control use the ELM-compatible AT commands (honoured by STN).
                string txHex = is29Bit ? txId.ToString("X8") : txId.ToString("X3");
                string rxHex = is29Bit ? rxId.ToString("X8") : rxId.ToString("X3");
                SendCommandChecked("ATSH" + txHex);
                SendCommandChecked("ATCRA" + rxHex);
                ConfigureFlowControlDefaults(txId);
                _currentTxId = txId;
                _currentRxId = rxId;

                RaiseLog($"OBDLink STP{preset}: {busLabel} ISO 15765 {(is29Bit ? "29" : "11")}-bit | TX 0x{txHex} | RX 0x{rxHex}.");
                return true;
            }
        }

        // ──────────────────────────────────────────────────────────────────────────────────────
        //  Raw-CAN monitor (CAN sniffer) — OBDLink/STN only. Uses a raw ISO 11898 preset + STMA to
        //  stream every frame on the bus. A generic ELM327 cannot reliably monitor raw CAN, so these
        //  return false/empty on a non-STN adapter. All STN CAN presets receive both 11- and 29-bit
        //  frames; the per-frame extended flag is inferred from the printed ID width.
        // ──────────────────────────────────────────────────────────────────────────────────────

        /// <summary>Start continuous raw-CAN monitoring on the given bus (OBDLink/STN only). Returns
        /// false on a generic ELM327. Pair with <see cref="ReadCanMonitorFrames"/> / <see cref="StopCanMonitor"/>.</summary>
        public bool StartCanMonitor(J2534Manager.CanBusKind bus, bool prefer29Bit = false)
        {
            lock (_portLock)
            {
                if (!_isOpen) return false;
                if (!_isStn) { RaiseLog("CAN monitor (sniffer) requires an OBDLink/STN adapter — a generic ELM327 cannot reliably monitor raw CAN."); return false; }
                if (_monitorActive) return true;

                int preset; string busLabel;
                switch (bus)
                {
                    case J2534Manager.CanBusKind.MsCan: preset = prefer29Bit ? 52 : 51; busLabel = "MS-CAN 125k"; break;
                    case J2534Manager.CanBusKind.SwCan: preset = prefer29Bit ? 62 : 61; busLabel = "SW-CAN 33.3k"; break;
                    default:                            preset = prefer29Bit ? 32 : 31; busLabel = "HS-CAN 500k"; break;
                }
                string r = SendCommandRaw("STP" + preset, 1500);
                if (r == null || IsErrorResponse(r)) { RaiseError($"CAN monitor: STP{preset} ({busLabel}) rejected by adapter."); return false; }
                SendCommandChecked("ATCAF0");   // raw frames — no ISO-TP reassembly
                SendCommandChecked("ATH1");     // CAN ID on
                SendCommandChecked("ATS1");     // spaces on — token-separated id/data for parsing

                _monBuf.Clear();
                try { if (_port.BytesToRead > 0) _port.DiscardInBuffer(); } catch { }
                _port.Write("STMA\r");          // monitor all frames, continuous until a byte is sent
                _monitorActive = true;
                RaiseLog($"OBDLink CAN monitor started: {busLabel} (STP{preset} + STMA).");
                return true;
            }
        }

        /// <summary>Drain monitored raw-CAN frames since the last call (non-blocking). Empty if the
        /// monitor is not running. Keeps any trailing partial line buffered for the next call.</summary>
        public List<RawCANFrame> ReadCanMonitorFrames(int maxFrames = 64)
        {
            var frames = new List<RawCANFrame>();
            lock (_portLock)
            {
                if (!_monitorActive || _port == null || !_port.IsOpen) return frames;
                try { if (_port.BytesToRead > 0) _monBuf.Append(_port.ReadExisting()); }
                catch { return frames; }

                string buf = _monBuf.ToString();
                int nl;
                while (frames.Count < maxFrames && (nl = buf.IndexOf('\r')) >= 0)
                {
                    string line = buf.Substring(0, nl).Trim();
                    buf = buf.Substring(nl + 1);
                    if (line.Length == 0 || line.Contains(">") || line.Contains("?") ||
                        line.IndexOf("STOPPED", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        line.IndexOf("BUFFER", StringComparison.OrdinalIgnoreCase) >= 0)
                        continue;
                    var f = ParseMonitorLine(line);
                    if (f.HasValue) frames.Add(f.Value);
                }
                _monBuf.Clear();
                _monBuf.Append(buf);   // retain the trailing partial line
            }
            return frames;
        }

        /// <summary>Stop raw-CAN monitoring and restore normal request mode (ISO-TP auto-format, spaces off).</summary>
        public void StopCanMonitor()
        {
            lock (_portLock)
            {
                if (!_monitorActive) return;
                _monitorActive = false;
                try
                {
                    _port.Write("\r");   // any byte interrupts STMA
                    var sb = new StringBuilder();
                    var deadline = DateTime.Now.AddMilliseconds(800);
                    while (DateTime.Now < deadline)
                    {
                        if (_port.BytesToRead > 0) { sb.Append(_port.ReadExisting()); if (sb.ToString().Contains(">")) break; }
                        else Thread.Sleep(5);
                    }
                }
                catch { }
                _monBuf.Clear();
                SendCommandChecked("ATCAF1");   // restore ISO-TP auto-formatting
                SendCommandChecked("ATS0");     // restore compact (spaces-off) output for requests
                RaiseLog("OBDLink CAN monitor stopped.");
            }
        }

        /// <summary>Parse one STN monitor line ("&lt;id&gt; &lt;d0&gt; &lt;d1&gt; …", spaces on). The STN prints
        /// the whole CAN ID as the first token (3 hex digits = 11-bit, longer = 29-bit extended).</summary>
        private static RawCANFrame? ParseMonitorLine(string line)
        {
            var tok = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (tok.Length < 1) return null;
            try
            {
                uint id = (uint)Convert.ToInt64(tok[0], 16);
                bool ext = tok[0].Length > 3;
                var data = new byte[tok.Length - 1];
                for (int i = 1; i < tok.Length; i++) data[i - 1] = (byte)Convert.ToInt32(tok[i], 16);
                return new RawCANFrame { CanId = id, Data = data, IsExtended = ext, IsTxEcho = false, Timestamp = 0 };
            }
            catch { return null; }
        }

        /// <summary>
        /// Set K-Line protocol — selects between ISO 9141-2 (ATSP3) and KWP2000 fast-init
        /// (ATSP5) using the supplied <see cref="ProtocolID"/>. Previous version picked the
        /// ATSP number from baudRate (≤5000 → ISO9141), which was wrong: 5-baud-INIT runs
        /// the bus at 10400 baud after init completes, so the baud-rate heuristic mis-selected
        /// fast-init for 5-baud targets.
        /// </summary>
        public bool SetProtocolKLine(uint baudRate = 10400, ProtocolID protocol = ProtocolID.ISO9141)
        {
            lock (_portLock)
            {
                if (!_isOpen) return false;

                // Mapping (per ELM327 datasheet §3):
                //   ATSP3 = ISO 9141-2 (5-baud init, KW1=08 / 94)         → ProtocolID.ISO9141
                //   ATSP4 = KWP2000 5-baud init                           → not exposed at J2534 level
                //   ATSP5 = KWP2000 fast init (25/25 ms wake pulse)       → ProtocolID.ISO14230
                int proto = (protocol == ProtocolID.ISO14230) ? ELM_PROTO_KWP2000_FAST : ELM_PROTO_ISO9141;
                SendCommandChecked("ATSP" + proto);

                // Some K-Line targets use non-default baud (Ford 4800, GM 9600). ATIB lets us
                // pick from the three rates the ELM supports. Defaults to 10400 if unspecified.
                if (baudRate == 4800)       SendCommandChecked("ATIB48");
                else if (baudRate == 9600)  SendCommandChecked("ATIB96");
                else                        SendCommandChecked("ATIB10");

                _currentProtocol = protocol;
                RaiseLog($"ELM327 protocol: K-Line (ATSP{proto}, {protocol}) at {baudRate} baud");
                return true;
            }
        }

        /// <summary>Set J1850 VPW protocol (GM).</summary>
        public bool SetProtocolVPW()
        {
            lock (_portLock)
            {
                if (!_isOpen) return false;
                SendCommandChecked("ATSP2");
                _currentProtocol = ProtocolID.J1850VPW;
                RaiseLog("ELM327 protocol: J1850 VPW (ATSP2)");
                return true;
            }
        }

        /// <summary>Set J1850 PWM protocol (Ford).</summary>
        public bool SetProtocolPWM()
        {
            lock (_portLock)
            {
                if (!_isOpen) return false;
                SendCommandChecked("ATSP1");
                _currentProtocol = ProtocolID.J1850PWM;
                RaiseLog("ELM327 protocol: J1850 PWM (ATSP1)");
                return true;
            }
        }

        /// <summary>
        /// Switch to a J2534 protocol by mapping ProtocolID to ELM327 ATSP number.
        /// </summary>
        public bool SetProtocol(ProtocolID protocol, uint baudRate, uint txId = 0, uint rxId = 0)
        {
            switch (protocol)
            {
                case ProtocolID.ISO15765:
                    return baudRate <= 300000
                        ? SetProtocolCAN250(txId, rxId)
                        : SetProtocolCAN(txId, rxId);

                case ProtocolID.ISO9141:
                    return SetProtocolKLine(baudRate, ProtocolID.ISO9141);

                case ProtocolID.ISO14230:
                    return SetProtocolKLine(baudRate, ProtocolID.ISO14230);

                case ProtocolID.J1850VPW:
                    return SetProtocolVPW();

                case ProtocolID.J1850PWM:
                    return SetProtocolPWM();

                case ProtocolID.CAN:
                    return SetProtocolCAN(txId, rxId);

                default:
                    RaiseError($"ELM327: Protocol {protocol} not supported");
                    return false;
            }
        }

        /// <summary>Set CAN at 250kbaud (11-bit).</summary>
        private bool SetProtocolCAN250(uint txId, uint rxId)
        {
            lock (_portLock)
            {
                if (!_isOpen) return false;
                SendCommandChecked("ATSP8");

                if (txId > 0)
                {
                    SendCommandChecked("ATSH" + txId.ToString("X3"));
                    _currentTxId = txId;
                }
                if (rxId > 0)
                {
                    SendCommandChecked("ATCRA" + rxId.ToString("X3"));
                    _currentRxId = rxId;
                }

                _currentProtocol = ProtocolID.ISO15765;
                RaiseLog($"ELM327 protocol: CAN 11-bit 250k");
                return true;
            }
        }

        /// <summary>Update transmit header and receive filter.</summary>
        public bool SetHeaders(uint txId, uint rxId)
        {
            lock (_portLock)
            {
                if (!_isOpen) return false;

                string txHex = txId.ToString("X3");
                string rxHex = rxId.ToString("X3");

                SendCommandChecked("ATSH" + txHex);
                SendCommandChecked("ATCRA" + rxHex);
                ConfigureFlowControlDefaults(txId);

                _currentTxId = txId;
                _currentRxId = rxId;

                RaiseLog($"ELM327 headers updated: TX=0x{txHex} RX=0x{rxHex}");
                return true;
            }
        }

        /// <summary>
        /// Configure ELM327 flow-control behaviour for the current TX header. Without this
        /// the default FC source ID is whatever ATSH was set to BEFORE the most recent
        /// command, which can be wrong (and silently truncates multi-frame RX on some
        /// strict ECUs). Sets:
        ///   ATFC SH &lt;tx&gt;     — FC source ID matches our request header
        ///   ATFC SD 30 00 00 — ContinueToSend, BS=0 (no block limit), STmin=0 (no delay)
        ///   ATFC SM 1        — use the SD/SH values we just set
        /// Errors are tolerated — older firmware / cheap clones may reject the FC commands;
        /// the library still works, it just falls back to the ELM's default FC behaviour.
        /// </summary>
        private void ConfigureFlowControlDefaults(uint txId)
        {
            try
            {
                string txHex = txId.ToString("X3");
                SendCommandRaw("ATFCSH" + txHex, 500);
                SendCommandRaw("ATFCSD300000", 500);
                SendCommandRaw("ATFCSM1", 500);
            }
            catch { /* clones may reject — non-fatal, defaults still apply */ }
        }

        #endregion

        #region Send / Receive

        /// <summary>
        /// Send a CAN ISO-TP message and receive the response.
        /// Equivalent to J2534Manager.SendReceive().
        /// Returns the response payload bytes (without CAN ID or PCI bytes), or null on timeout/error.
        /// </summary>
        public byte[] SendReceive(uint txId, byte[] payload, uint timeout = 2000)
        {
            lock (_portLock)
            {
                if (!_isOpen) return null;

                try
                {
                    // If the TX ID has changed, update the header
                    if (txId != _currentTxId)
                    {
                        string txHex = txId.ToString("X3");
                        SendCommandRaw("ATSH" + txHex, 500);
                        _currentTxId = txId;
                    }

                    // Build hex string from payload bytes (e.g. {0x22, 0xF1, 0x90} -> "22F190")
                    string hexPayload = BytesToHex(payload);

                    string response;
                    // LONG-REQUEST PATH (STN / OBDLink only). A classic ELM327 cannot ISO-TP-segment a
                    // request larger than one frame: the AT command line carries the whole message and the
                    // chip only auto-formats a single frame, so long 0x2E writes and multi-DID 0x22 silently
                    // fail (the ELM ceiling). STN chips expose STPX, which performs ISO-TP TX segmentation of
                    // an arbitrary-length payload using the ATSH header set above (CAF1 applies the framing).
                    // Gated to >7 payload bytes so the proven short-request path is byte-for-byte unchanged.
                    // ⚠ BENCH-PENDING: validate the STPX round trip on real STN iron — not verifiable here.
                    if (IsStn && payload.Length > 7)
                    {
                        string stpx = "STPX d:" + hexPayload;
                        RaiseLog($"ELM327(STN) long request via STPX ({payload.Length} bytes): {hexPayload}");
                        response = SendCommandRaw(stpx, (int)timeout);
                    }
                    else
                    {
                        // Short / single-frame request — original proven path.
                        response = SendCommandRaw(hexPayload, (int)timeout);
                    }

                    if (response == null)
                    {
                        RaiseLog($"ELM327 TX: {hexPayload} → (no response)");
                        return null;
                    }

                    // Parse the response (strips CAN ID header and ISO-TP PCI bytes)
                    byte[] parsed = ParseResponse(response);
                    if (parsed != null)
                        RaiseLog($"ELM327 TX: {hexPayload} → RX: {BytesToHex(parsed)} ({parsed.Length} bytes)");
                    else
                        RaiseLog($"ELM327 TX: {hexPayload} → RX: (parse failed) raw='{response.Replace("\r", "\\r").Replace("\n", "\\n")}'");
                    return parsed;
                }
                catch (Exception ex)
                {
                    RaiseError("ELM327 SendReceive error: " + ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// Send a CAN message with 29-bit extended addressing.
        /// Temporarily switches to ATSP7 if not already in 29-bit mode.
        /// </summary>
        public byte[] SendReceive29Bit(uint txId, byte[] payload, uint timeout = 3000)
        {
            lock (_portLock)
            {
                if (!_isOpen) return null;

                // Snapshot the prior 11-bit CAN state so we can restore it — otherwise the
                // adapter stays on ATSP7 and a following 11-bit SendReceive breaks.
                bool wasAlready29 = _is29Bit;
                uint savedTx = _currentTxId, savedRx = _currentRxId;
                try
                {
                    // Ensure 29-bit protocol
                    SendCommandRaw("ATSP7", 500);
                    _is29Bit = true;

                    string txHex = txId.ToString("X8");
                    SendCommandRaw("ATSH" + txHex, 500);
                    _currentTxId = txId;

                    string hexPayload = BytesToHex(payload);
                    string response = SendCommandRaw(hexPayload, (int)timeout);

                    if (response == null)
                        return null;

                    return ParseResponse(response);
                }
                catch (Exception ex)
                {
                    RaiseError("ELM327 SendReceive29Bit error: " + ex.Message);
                    return null;
                }
                finally
                {
                    // Restore the prior 11-bit CAN protocol + header if that's where we came from,
                    // so a following 11-bit call isn't left stranded on ATSP7.
                    if (!wasAlready29)
                    {
                        try
                        {
                            SendCommandRaw("ATSP6", 500);
                            SendCommandRaw("ATSH" + savedTx.ToString("X3"), 200);
                            SendCommandRaw("ATCRA" + savedRx.ToString("X3"), 200);
                            _currentTxId = savedTx; _currentRxId = savedRx; _is29Bit = false;
                        }
                        catch { }
                    }
                }
            }
        }

        /// <summary>
        /// Send a K-Line message and receive response.
        /// Payload is the raw ISO 9141 / KWP2000 message bytes.
        /// </summary>
        public byte[] SendReceiveKLine(byte[] payload, uint timeout = 2000)
        {
            lock (_portLock)
            {
                if (!_isOpen) return null;

                try
                {
                    string hexPayload = BytesToHex(payload);
                    string response = SendCommandRaw(hexPayload, (int)timeout);

                    if (response == null)
                        return null;

                    return ParseResponseKLine(response);
                }
                catch (Exception ex)
                {
                    RaiseError("ELM327 SendReceiveKLine error: " + ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// Send a J1850 VPW message and receive response.
        /// </summary>
        public byte[] SendReceiveVPW(byte[] payload, uint timeout = 2000)
        {
            lock (_portLock)
            {
                if (!_isOpen) return null;

                try
                {
                    string hexPayload = BytesToHex(payload);
                    string response = SendCommandRaw(hexPayload, (int)timeout);

                    if (response == null)
                        return null;

                    return ParseResponseRaw(response);
                }
                catch (Exception ex)
                {
                    RaiseError("ELM327 SendReceiveVPW error: " + ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// Attempt to read any pending response from the serial buffer.
        /// Used by UDSManager for NRC 0x78 (ResponsePending) follow-up reads.
        /// Note: ELM327 typically handles 0x78 internally and returns the final response,
        /// so this is mainly a safety net for edge cases.
        /// </summary>
        public byte[] ReadPendingResponse(uint timeout = 2000)
        {
            lock (_portLock)
            {
                if (!_isOpen) return null;

                try
                {
                    StringBuilder sb = new StringBuilder();
                    DateTime deadline = DateTime.Now.AddMilliseconds(timeout);

                    while (DateTime.Now < deadline)
                    {
                        if (_port.BytesToRead > 0)
                        {
                            string data = _port.ReadExisting();
                            sb.Append(data);
                            if (data.Contains(">"))
                                break;
                        }
                        Thread.Sleep(10);
                    }

                    string response = sb.ToString().Trim();
                    if (string.IsNullOrEmpty(response) || IsErrorResponse(response))
                        return null;

                    return ParseResponse(response);
                }
                catch
                {
                    return null;
                }
            }
        }

        #endregion

        #region Battery Voltage

        /// <summary>Read battery voltage from ELM327 (AT RV command).</summary>
        public double ReadBatteryVoltage()
        {
            lock (_portLock)
            {
                if (!_isOpen) return 0;
                // While the raw-CAN monitor (STMA) is streaming, DO NOT issue ATRV: any byte sent to the
                // adapter stops the monitor, and SendCommandRaw flushes the RX buffer (dropping queued frames),
                // which silently kills the sniffer. Return the last known voltage so the sniffer keeps running.
                if (_monitorActive) return _lastBatteryVolts;

                try
                {
                    string response = SendCommandRaw("ATRV", 1000);
                    if (response == null) return 0;

                    // Response format: "12.6V" or "12.6"
                    string cleaned = response.Replace("V", "").Replace("v", "").Trim();

                    // Find the voltage value (may have other text around it)
                    foreach (string part in cleaned.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string trimmed = part.Trim();
                        if (double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out double voltage))
                        {
                            if (voltage > 0 && voltage < 30) // Sanity check
                            { _lastBatteryVolts = voltage; return voltage; }
                        }
                    }
                    return 0;
                }
                catch
                {
                    return 0;
                }
            }
        }

        #endregion

        #region Tester Present

        /// <summary>
        /// Start sending TesterPresent (3E 80) at the specified interval.
        /// Uses a software timer since ELM327 periodic messages are limited.
        /// </summary>
        public void StartTesterPresent(uint txId, uint intervalMs = 2000)
        {
            StopTesterPresent();
            _tpTxId = txId;
            _tpActive = true;

            _testerPresentTimer = new Timer(TesterPresentCallback, null, intervalMs, intervalMs);
            RaiseLog($"ELM327 TesterPresent started: 0x{txId:X3} every {intervalMs}ms");
        }

        /// <summary>Stop the TesterPresent timer.</summary>
        public void StopTesterPresent()
        {
            _tpActive = false;
            if (_testerPresentTimer != null)
            {
                _testerPresentTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _testerPresentTimer.Dispose();
                _testerPresentTimer = null;
            }
        }

        private void TesterPresentCallback(object state)
        {
            if (!_tpActive || !_isOpen) return;

            // Skip this TP cycle if the port is busy (don't block ongoing transfers)
            if (!Monitor.TryEnter(_portLock, 0))
                return;
            try
            {
                if (!_isOpen || _port == null || !_port.IsOpen) return;

                // If the TX ID differs from the current header, update it
                if (_tpTxId != _currentTxId)
                {
                    string txHex = _tpTxId.ToString("X3");
                    SendCommandRaw("ATSH" + txHex, 500);
                    _currentTxId = _tpTxId;
                }

                // Send 3E 80 (TesterPresent, suppress positive response)
                SendCommandRaw(BytesToHex(new byte[] { 0x3E, 0x80 }), 500);
            }
            catch { /* Silently ignore TP errors */ }
            finally
            {
                Monitor.Exit(_portLock);
            }
        }

        #endregion

        #region Low-Level Serial Communication

        /// <summary>
        /// Send a command string to the ELM327 and read the full response up to the '>'
        /// prompt or timeout.  Public alias of SendCommandRaw for the developer console
        /// (Developer menu → ELM327 AT command tester).
        /// </summary>
        public string SendAtCommand(string command, int timeoutMs = 2000)
            => SendCommandRaw(command, timeoutMs);

        /// <summary>
        /// Send a command string to the ELM327 and read the full response
        /// up to the '>' prompt or timeout.
        /// </summary>
        private string SendCommandRaw(string command, int timeoutMs = 2000)
        {
            if (_port == null || !_port.IsOpen)
                return null;

            try
            {
                // Drain any pending data
                if (_port.BytesToRead > 0)
                    _port.DiscardInBuffer();

                // Send command with CR terminator
                _port.Write(command + "\r");

                // Read response until '>' prompt. The ELM emits "SEARCHING..." while it
                // auto-detects the protocol on the first packet — that can legitimately take
                // 5+ seconds even on a working bus. When we see SEARCHING we extend the
                // deadline by another 4 s instead of returning null prematurely.
                StringBuilder sb = new StringBuilder();
                DateTime deadline = DateTime.Now.AddMilliseconds(timeoutMs);
                bool searchingExtended = false;

                while (DateTime.Now < deadline)
                {
                    if (_port.BytesToRead > 0)
                    {
                        string chunk = _port.ReadExisting();
                        sb.Append(chunk);

                        // '>' is the ELM327 ready prompt
                        if (chunk.Contains(">"))
                            break;

                        // Extend deadline if the adapter is mid-protocol-search. One-shot
                        // extension — if it searches twice, the bus is genuinely dead.
                        if (!searchingExtended &&
                            sb.ToString().IndexOf("SEARCHING", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            deadline = DateTime.Now.AddMilliseconds(4000);
                            searchingExtended = true;
                        }
                    }
                    else
                    {
                        Thread.Sleep(5);
                    }
                }

                string response = sb.ToString();

                // Strip the command echo if present (shouldn't be with ATE0, but safety)
                if (response.StartsWith(command, StringComparison.OrdinalIgnoreCase))
                    response = response.Substring(command.Length);

                // Strip the '>' prompt
                response = response.Replace(">", "").Trim();

                return response;
            }
            catch (TimeoutException)
            {
                return null;
            }
            catch (Exception ex)
            {
                RaiseError($"ELM327 serial error: {ex.Message}");
                return null;
            }
        }

        /// <summary>Send an AT command and verify OK response.</summary>
        private bool SendCommandChecked(string atCommand)
        {
            string response = SendCommandRaw(atCommand, 1500);
            if (response == null)
            {
                RaiseError($"ELM327 command timeout: {atCommand}");
                return false;
            }

            if (response.Contains("?") || response.Contains("ERROR"))
            {
                RaiseError($"ELM327 command rejected: {atCommand} -> {response}");
                return false;
            }

            return true;
        }

        #endregion

        #region Response Parsing

        /// <summary>
        /// Parse an ELM327 CAN response into payload bytes.
        /// With ATH1 (headers on) and ATS0 (spaces off), CAN responses look like:
        ///   "7E80362F190" (3-digit header + PCI byte + data bytes)
        /// Strips the CAN ID header and ISO-TP PCI bytes so the returned payload matches
        /// the J2534 native path (pure UDS/OBD2 payload only).
        /// Handles both genuine ELM327 (ATCAF1 strips PCI) and clones (PCI bytes present).
        /// Filters responses by _currentRxId to reject data from unexpected ECUs.
        /// </summary>
        private byte[] ParseResponse(string raw)
        {
            if (string.IsNullOrEmpty(raw))
                return null;

            // Check for error responses
            if (IsErrorResponse(raw))
                return null;

            // Split into lines (multi-frame responses may span lines)
            string[] lines = raw.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Build the expected RX CAN ID prefix for filtering. 11-bit IDs render as 3
            // hex chars ("7E8"); 29-bit as 8 ("18DAF110"). Stripping a fixed 3 corrupts
            // every 29-bit response, so size the header off the ID magnitude.
            bool is29Bit = _currentRxId > 0x7FF;
            int canHeaderHexChars = is29Bit ? 8 : 3;
            string expectedRxPrefix = _currentRxId > 0 ? _currentRxId.ToString(is29Bit ? "X8" : "X3") : null;

            // Collect hex payload strings after stripping CAN ID
            List<string> payloadHexLines = new List<string>();
            foreach (string line in lines)
            {
                string trimmed = line.Trim();

                // Skip empty, prompt, or status lines
                if (string.IsNullOrEmpty(trimmed) || trimmed == ">" || trimmed == "OK" ||
                    trimmed.StartsWith("SEARCHING") || trimmed.StartsWith("BUS INIT"))
                    continue;

                // Skip AT command echoes
                if (trimmed.StartsWith("AT", StringComparison.OrdinalIgnoreCase))
                    continue;

                // This should be hex data — validate
                if (IsHexString(trimmed))
                {
                    // With ATH1, first 3 characters are the CAN ID (e.g. "7E8")
                    // For CAN, strip the 3-character header to get the payload
                    if (_currentProtocol == ProtocolID.ISO15765 || _currentProtocol == ProtocolID.CAN)
                    {
                        if (trimmed.Length > canHeaderHexChars)
                        {
                            // Filter by expected RX CAN ID — reject responses from other ECUs
                            if (expectedRxPrefix != null &&
                                !trimmed.StartsWith(expectedRxPrefix, StringComparison.OrdinalIgnoreCase))
                            {
                                System.Diagnostics.Debug.WriteLine(
                                    $"ELM327 ParseResponse: skipping unexpected CAN ID '{trimmed.Substring(0, Math.Min(canHeaderHexChars, trimmed.Length))}' (expected {expectedRxPrefix})");
                                continue;
                            }

                            // Strip the CAN ID header (3 hex chars for 11-bit, 8 for 29-bit);
                            // keep the rest (may still contain PCI bytes).
                            payloadHexLines.Add(trimmed.Substring(canHeaderHexChars));
                        }
                    }
                    else
                    {
                        payloadHexLines.Add(trimmed);
                    }
                }
            }

            if (payloadHexLines.Count == 0)
                return null;

            // For CAN ISO-TP, detect and strip PCI bytes if present
            if (_currentProtocol == ProtocolID.ISO15765 || _currentProtocol == ProtocolID.CAN)
                return AssembleIsoTpPayload(payloadHexLines);

            // Non-CAN protocols: concatenate raw hex
            return HexToBytes(string.Concat(payloadHexLines));
        }

        /// <summary>
        /// Assemble ISO-TP payload from hex data lines (CAN ID already stripped).
        /// Detects whether PCI bytes are present (some ELM327 clones include them) or
        /// already stripped (genuine ELM327 with ATCAF1) and handles both cases.
        ///
        /// PCI byte detection:
        ///   - Single frame PCI = 0x01–0x07 (upper nibble 0, lower = data length)
        ///   - First frame PCI  = 0x10–0x1F (upper nibble 1, multi-frame start)
        ///   - These never conflict with valid UDS/OBD2 response SIDs (0x41+, 0x7F)
        /// </summary>
        private byte[] AssembleIsoTpPayload(List<string> hexLines)
        {
            if (hexLines.Count == 0) return null;

            string firstLine = hexLines[0];
            if (firstLine.Length < 2)
                return HexToBytes(string.Concat(hexLines));

            byte firstByte;
            if (!byte.TryParse(firstLine.Substring(0, 2), NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture, out firstByte))
                return HexToBytes(string.Concat(hexLines));

            int frameType = (firstByte >> 4) & 0x0F;
            int lowerNibble = firstByte & 0x0F;

            // --- Single Frame with PCI byte present (0x01–0x07) ---
            if (frameType == 0 && lowerNibble >= 1 && lowerNibble <= 7)
            {
                int dataLen = lowerNibble;
                string dataHex = firstLine.Substring(2); // skip 1 PCI byte
                if (dataHex.Length > dataLen * 2)
                    dataHex = dataHex.Substring(0, dataLen * 2);
                RaiseLog($"ISO-TP: stripped SF PCI byte (len={dataLen})");
                return HexToBytes(dataHex);
            }

            // --- First Frame: multi-frame with PCI bytes (0x10–0x1F) ---
            if (frameType == 1)
            {
                if (firstLine.Length < 4)
                    return HexToBytes(string.Concat(hexLines));

                byte secondPciByte;
                if (!byte.TryParse(firstLine.Substring(2, 2), NumberStyles.HexNumber,
                        CultureInfo.InvariantCulture, out secondPciByte))
                    return HexToBytes(string.Concat(hexLines));

                int totalLen = (lowerNibble << 8) | secondPciByte;

                StringBuilder assembled = new StringBuilder();
                // First frame: skip 2 PCI bytes (4 hex chars)
                assembled.Append(firstLine.Substring(4));

                // Consecutive frames: skip 1 PCI byte (2 hex chars) each
                for (int i = 1; i < hexLines.Count; i++)
                {
                    string cfLine = hexLines[i];
                    if (cfLine.Length >= 2)
                    {
                        byte cfByte;
                        if (byte.TryParse(cfLine.Substring(0, 2), NumberStyles.HexNumber,
                                CultureInfo.InvariantCulture, out cfByte) && (cfByte >> 4) == 2)
                        {
                            // Consecutive frame — strip PCI byte
                            assembled.Append(cfLine.Substring(2));
                        }
                        else
                        {
                            // Not a CF PCI byte — include as-is
                            assembled.Append(cfLine);
                        }
                    }
                }

                // Trim to totalLen bytes
                string hexData = assembled.ToString();
                if (hexData.Length > totalLen * 2)
                    hexData = hexData.Substring(0, totalLen * 2);

                RaiseLog($"ISO-TP: stripped FF/CF PCI bytes (totalLen={totalLen}, frames={hexLines.Count})");
                return HexToBytes(hexData);
            }

            // --- PCI bytes already stripped (genuine ELM327 ATCAF1, or response SID >= 0x40) ---
            return HexToBytes(string.Concat(hexLines));
        }

        /// <summary>
        /// Parse a K-Line response (ISO 9141-2 / KWP2000) — strips the ISO 14230 header
        /// and trailing checksum so the caller gets the bare service payload.
        ///
        /// HEADER FORMAT (ISO 14230-2 §5.1):
        ///   Format byte high 2 bits decode addressing mode:
        ///     00xxxxxx  → ISO 9141-2 default (header = fmt + tgt + src, 3 bytes)
        ///     01xxxxxx  → physical addressing, no tgt/src   (header = fmt only, 1 byte)
        ///     10xxxxxx  → with tgt/src; LOWER 6 bits hold length
        ///                  if length-bits = 0 → length is in 4th byte (header = 4 bytes)
        ///                  if length-bits ≠ 0 → length embedded in fmt (header = 3 bytes)
        ///     11xxxxxx  → reserved
        ///
        /// The previous implementation hard-coded a 3-byte header strip, which mis-parsed
        /// any KWP2000 long message (≥64 data bytes — fmt = 0x80 + separate length byte).
        /// </summary>
        private byte[] ParseResponseKLine(string raw)
        {
            if (string.IsNullOrEmpty(raw) || IsErrorResponse(raw))
                return null;

            string[] lines = raw.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder hexData = new StringBuilder();

            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed == ">" || trimmed == "OK" ||
                    trimmed.StartsWith("SEARCHING") || trimmed.StartsWith("BUS INIT"))
                    continue;
                if (trimmed.StartsWith("AT", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!IsHexString(trimmed) || trimmed.Length < 4)
                {
                    if (IsHexString(trimmed)) hexData.Append(trimmed);
                    continue;
                }

                // Parse the format byte to decide header length.
                byte fmt;
                if (!byte.TryParse(trimmed.Substring(0, 2), NumberStyles.HexNumber,
                                   CultureInfo.InvariantCulture, out fmt))
                {
                    hexData.Append(trimmed);
                    continue;
                }

                int upperBits = (fmt >> 6) & 0x03;
                int embeddedLen = fmt & 0x3F;
                int headerHexChars;

                if (upperBits == 0x02)        // 10xxxxxx — KWP2000 with tgt/src + length
                    headerHexChars = (embeddedLen == 0) ? 8 : 6;   // 4-byte vs 3-byte header
                else if (upperBits == 0x01)   // 01xxxxxx — ISO 9141-2 fmt 0x48/0x68: 3-byte header (fmt + tgt + src)
                    headerHexChars = 6;       // was 2 — leaked the tgt+src bytes into the payload
                else                          // 00xxxxxx — ISO 9141-2 default
                    headerHexChars = 6;       // fmt + tgt + src

                // Need room for at least header + checksum (1 byte = 2 hex chars).
                if (trimmed.Length <= headerHexChars + 2)
                {
                    hexData.Append(trimmed);   // line too short for our model — pass through
                    continue;
                }

                // Strip the header AND the trailing 1-byte checksum.
                int dataChars = trimmed.Length - headerHexChars - 2;
                hexData.Append(trimmed.Substring(headerHexChars, dataChars));
            }

            return hexData.Length > 0 ? HexToBytes(hexData.ToString()) : null;
        }

        /// <summary>Parse a raw protocol response (J1850 etc.) — return all data bytes.</summary>
        private byte[] ParseResponseRaw(string raw)
        {
            if (string.IsNullOrEmpty(raw) || IsErrorResponse(raw))
                return null;

            string[] lines = raw.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder hexData = new StringBuilder();

            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed == ">" || trimmed == "OK" ||
                    trimmed.StartsWith("SEARCHING") || trimmed.StartsWith("BUS INIT"))
                    continue;
                if (trimmed.StartsWith("AT", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (IsHexString(trimmed))
                    hexData.Append(trimmed);
            }

            return hexData.Length > 0 ? HexToBytes(hexData.ToString()) : null;
        }

        /// <summary>Check if the response indicates an error condition.</summary>
        private bool IsErrorResponse(string response)
        {
            if (response == null) return true;
            string upper = response.ToUpperInvariant();

            // "?" alone on a line means "command not understood". Detect it as a token,
            // not as a substring — some valid hex data could legitimately contain '?'
            // if the parser is confused, but on a clean response a bare "?" is always
            // an error. Check each line separately.
            foreach (var line in upper.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string t = line.Trim();
                if (t == "?") return true;
            }

            return upper.Contains("NO DATA") ||
                   upper.Contains("UNABLE TO CONNECT") ||
                   upper.Contains("BUS ERROR") ||
                   upper.Contains("BUS BUSY") ||                       // ELM bus-busy — common on cheap dongles
                   upper.Contains("DATA ERROR") ||
                   upper.Contains("BUFFER FULL") ||
                   upper.Contains("CAN ERROR") ||
                   upper.Contains("FB ERROR") ||
                   upper.Contains("FC RX TIMEOUT") ||                  // ECU never sent flow control
                   upper.Contains("<RX ERROR") ||
                   upper.Contains("LP ALERT") ||
                   upper.Contains("LV RESET") ||
                   upper.Contains("ACT ALERT") ||
                   upper.Contains("STOPPED") ||
                   upper.Contains("ERR");                              // "ERR94", "ERRxx" generic ELM errors
        }

        #endregion

        #region Utility Methods

        /// <summary>Convert a byte array to a hex string without spaces.</summary>
        private static string BytesToHex(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return "";
            StringBuilder sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        /// <summary>Convert a hex string (no spaces) to a byte array.</summary>
        private static byte[] HexToBytes(string hex)
        {
            if (string.IsNullOrEmpty(hex)) return null;

            // Remove any remaining spaces or non-hex chars
            hex = new string(hex.Where(c => IsHexChar(c)).ToArray());

            // Must be even length
            if (hex.Length % 2 != 0)
                hex = "0" + hex;

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber);
            }
            return bytes;
        }

        /// <summary>Check if a string contains only valid hex characters.</summary>
        private static bool IsHexString(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            return s.All(c => IsHexChar(c));
        }

        private static bool IsHexChar(char c)
        {
            return (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
        }

        /// <summary>Extract the ELM327 version string from the ATZ response.</summary>
        private static string ExtractElmVersion(string atzResponse)
        {
            if (string.IsNullOrEmpty(atzResponse)) return "Unknown";

            // Look for "ELM327" in the response
            string[] lines = atzResponse.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (trimmed.Contains("ELM"))
                    return trimmed;
            }
            return "ELM327 (version unknown)";
        }

        private void RaiseLog(string msg) => OnLog?.Invoke(msg);
        private void RaiseError(string msg) => OnError?.Invoke(msg);

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                Close();
            }
        }

        #endregion
    }
    
} // +++*+++++++=====++***++++++****+==-:::-:.::-::::..--=+*#%#+=-:::.:::-:---.. ...:::-::::::::::::.:.:::::::-:::::::::::::::::::::::::::::::::::::::::::-::::::::::::::::::::::::::::::::::.::::::::::::-:::.. . .---:--:::-:---=---=+==:=--::.:::-=++****+**+++++***+++++++++++++++++++ 
 // ++++===+==+++++**+++*+++***+=--------::.-::::::==+*##+=-::::::::-:::: ....:::::::::.::::.:.:::::::::::::::::::::::::::::::::::::::::::::::::::.:::::::::::-:::::::::::::::::::::::::::::::::::.::::::::::-::::......:-=:---::::+===========--:--:---+=**++++++=+++***+++++++++**+***+ 
 // +=+=+=++++==+++*+++++***+=-:-:--.::.:-:-::-:--+*%*+--::.::---:.::....::.::::.:::..::::::.:::::::::::::::::::.::::.:::::::-::...::..::.:::::..::::::::::::::::::::::::::::::::::::::::-:::::::-::::::.:::::::::::::::. ..:-====-::-:==----==++==---:::-=+++**+**++++++***+++++==++++** 
 // ===++**==+**+*+**+***+=:-----::::-::.:.-==+++*+=:.:::--=-:...:..:::::::::::::::::::::::::::::::::::::::::::::-::::::.:.....:--:----:-:-:::--:-.::.:::-::::.:.::::::.:::::-:::-:-.:::::::::-::-:::::::::::::::.:::::-::.:... ..-=--:::------=-=+++:=-::::--=++**++++++++****++**=-++++ 
 // =+++===+**+++++***+=:--:-.:::--:::::-=-=+++=-:.:::::::  ....:.:::::::.::::::::.::::::::.::::::::::::::...::.::.::::::::------:..-::.:=-:::---::---:-:-:.----.:-::::--:::.:....::::::::.::::::::-::-::::::::::::::.::-:--::::... .--=--:::--------==+=----:----=+***+++++++*******==== 
 // ++==++*+++*****=--:---:--:-:::::--==+%+=-::---=--.. ..:::::::::.....:::::::::::::::.:.::-:::::-:::::.----.:.--:.:=-:.:---::---=:-:::::..:.::.:.:.:--:::--:::-=-:..-::..:----:-:::--::::-:::::::::::::::::::::-:::::::.:::::-:::::....:-----:.:---=====*+---::::-:-=++++++++++******+- 
 // +==+++++*#**+=-:::-::::::-:::-:=++++--:----:-. ...::::::::::..:.:::::::::::.:::..::::-:.:.::..: :..-:: -=-::-:-:=::.::..........:.:::::::::.:::::::.:::.:::::..::-::-=---::.=-:..-:::--:.:-.:-::::::::::::::::::::::::::::..:::::::::....:-----::------+++=---::---==+***++++++****#+ 
 // =+*+***#+=--::-:::---:=:::.-=+---=-::::-:..  :.::::::...:...:::::::::::::::::.:-:.:::.:::-::..:=:  ...            . ...........:.:...    .:.:.:::::::.....           .   .....::::::.-:::.:::-:..--.::::::-::::::::::::::::::::.:::::::.....::---::-:--:-=-=*#+=-:::-=:=++**+++=+++*+ 
 // *****+=-::---..--::.:.::==-=-+==-::-=-:....:::::.:..::.::::::::::::.:::::::.-:::-::.:-:.  ::...--======+++++++++++==----:::::::::::--=##=:::::::::::::---====++++++===---::::...............   :::::-:::::..:::::::::-::::-::::::::::::::-:....:-----::-=-----===+-::..::-==+++*+==+= 
 // *+*++=---:::--::::-:-:==+++=::.::-:.. .::::.:::::::::::::::::::..:::::..::-::...=:....:-:--------------------=-----::::::.::::::::-::=*#=-:::::.:::::::.:::--------------=----====-=========++==    .....:-:::.:::::::::::::::-::::::::::-:-:::....:----::.-:---=**+*==:::::-==+====* 
 // *+==:-:::::-:-:::::=*==--::::-:....::::.:::....:::::::::::::::::.:..:.:.:..:--...:::--::::----:-:---==---:::........::.:::::.:::::...    .....::::::::::::::.::----=-==---=---------------=---==+**+=.. .....-:::::::::::::::::::::::::::.::::::::....:----:::::---=-+==--::.::-:-+** 
 // -::--=..:::.:::::--==--::--=:.:..::::::::.:.:::::::::::..::::.:::--::.:+=:.-::::::-::-----===++****+*+++++=+====-:::..:..:::.:.:::..:::....:::::.:.:.:.:::::---==++**#*****++++==+=====--------======+**##.: . ...-:::::::::.:::::::-:::::::::-:::::.:...:==--::.-:-=-#*+*+=---:-*%#* 
 // -:-::--::-:::::==+=--.:--:. :..::::::....::::::::::::::::.:::::...---:.....  ..:..........                     . ...:::::::::::-:::::::::=::::::::::::::.::....                          ...::.    .:-:.   :+*=-... ..---:.::::::::::::::::::::::::::::::.. .:+=--::--:-==-=---:==-.- 
 // -::.:::::.:--=+=-:::--:. .:..::.:.:..:::::::::::.:....:::..:..=-:..-......:.:.:...:.......:::-=========--------::::::::..::::..:.........:...:...::::.::::::::-------====++*+*##%*======---:::::::.. ..::-=---=+++*--.-....::::..::::::::::::::::.:::::::::... .:=--::-:-==*+##=--+=: 
 // :.:-:::-:-=+=-:::--::..:.::::::::.::::::::::.::..::::-:.:.+=:.......:::---:------::-:-:::::::::::::::::-:::::::::::.:::.:::::..:.:::::.:...:::.:.::::::::::::::::--------::::......::::::::::---=-===++==---::::..:.:::::.-:::.--::::::::::::::-::::.::::-:::::....:::::-::-==:::---: 
 // :.:..::=-+==:-:--.  .:.:::::::....::::::::.:.::::::..=--:.:....:::::::::.:::...:..:....:::::------------::::::.......................... @ .....................::.::---==============-----------------------------=-=-----::.:-::.:.:::::::::::::::::::::::::::.:...=-::....-=-=+=-= 
 // :=::====-:::::::..::::::::::.:.::::::.:.::......:---..::.:...:::::.:::::::.:::::::---=--==---:::...            . ..............::.:.:.:. % :::.:::::::::::..:....::...........::::::::------===================--------:---:----:..--::::::::::::::::::::.:.::::::::..:-:-..*+=---=*+ 
 // ::--=-::::--::...::::::....:::::::::.::::..----....:::...::::::::::::::::::::::::::::.......  ..::::::---==-=---------=---------::--:--: @ :---::--:-:-:--:-------------------:-:-::::::::::::.:::....................:::::::::::-:-:..::-:::::::::::::::-::::::::-::.. .:=++=--==--= 
 // -----:::=-. ..::.:.::...:::.::::::.::::.:::..:::::.:..:::.:...........:::::::::-:::::--.::::::::::::::-::-:-:--------=--=-==---=-====--= % ==-==-=---==----=-==--=-==-=-=------::.............:::----------------:-:::.......:::::::::=-:...:::::::::::::::::::::::::::--::::.::--==- 
 // ---:-:::  .::::::::...:::::::::.::.....:.=--:........:....::::----------::::....:...    . ..::::=----:---::-::....... .               .  @                       .. ......    ....::--==-----:.....:::-====------------=----::::.::-....:=---:::::-:::::::::::::::.::------:::-:::::: 
 // ::.:=:..::::.::::.::::::-::.......::-+::::......:---------:-::::-:--:::....::::---=-=-=--::..         ...........::.::::::::::::::--::::.:.::::::-:::::::::-:::::----:.   .:-==========:+=++=+====--:.   .::-------===---::::---:.::.:::...:-:.:--::::::::::::::::::::........::::.-- 
 // :-=. ..:::::::..:.::::::.:....::-:-: ..::....:::::..::::::-------:.....:----=======:+=+===+=++++-:    .:::::::::::::::::::::::::::::::::::.:::::::::.::::::::::::-:.   -+++=+++#%%@@@@@@@@@@@@%#*:++===-.   .:::......:---::-:::::--:.:::::...---...::::::::::::::--:.=..--..-.:.:::: 
 // -:...::::::.:.:::::.:.::..:-::-:-:-:-..:..::--:::-----:::......   .---==:---+=##@#@#@#@#@%@*%-=*****+:   .:::::::::::::::::::::::.-::::::-:::::::-:::::::::::::..  .-+****%*@@@@#-           :-@@@@%@*++++=:  ::------:.:-=-:---::--:..:::::....:----::::::::::::::.::::...::::..:::: 
 // :..:::::::...::.:::::....:-::::...:...::::::::-:--:..::::::::...-==---==##@**-.     .     :=@%@@@##+****:   .:::::::..:.......... .    .         .         . .   :=++**@@@@*     = .= +.*.=:..     -@@@@**=++: .:--:::::..:--::::::--:.:.:::::.::..:-..:::.::::.:::::::::-:.::.::.... 
 // .:::::.....:.:::::.....::::.::--.....::--::::::::.:::::::::..:===---**#+.      --.::* :.-.       #@@@##+*#+        ...-:----::--::=-----------==-==+++=-==---. -*#**@@@%    @.-#.+::* . # *.#-==:+ :   #@@##++=- .:------:.:---:-:--::::.:::::::::...:-=---::.::::::::::::::--::=:.-= 
 // .::::.:..:::::::..::.:-----::.......::::::::::::.::::::::..:==-:-+*#-   ..:@=.:=#.:.*.:.*-  -@.     =@@+%**#*: :-====-=:.  .=-====+=::--====---: .   .:-==-. .++-#@@%: . -* *  - ...=-+=+...:   #:=-:-   .@@@#*+=. .:--::--..--:::::::::.::::::::.::::. :....:.::::::::::::::........ 
 // ::.:.::::::.::.:::..:::.......:.::::::::-::-:-..:::----:..-----+#+.  .-%:: .-...::-:::-:-::. : :-@:    @@@%***+  .    --:-:.=-=-.    +     .:---.---:.      -+*#@@#    =+.= -=**+:::##++*:.+#@+*- ..#.+=.   @@@#*+=  ::-::--:.--:---:::::..:::::::::::..---::=-::.:::::::::::::-:.:+. 
 // .....:.:.::.:.:.::------:...:::::..:..:::::::.:::::::::.:-----*+  :*:...+..:-#-...:...:....=%#.:--.::*:  -@@#**#+.:--::    .=:   .::.@-:::. . :=     .-=-.:+**@@@ -* *+.:..*#-==:...     ..:.-=+: -+. .=#:-: :@@#*+=. ::::::-..--:-:::::::::::::::::::.:::-... .:::::::::::-::::::::. 
 // :....:::.:.:::.::.::.::...:...::.:::::::----::-::::-::.-=---+#.  ...*-:...:.:=  ..:.:.::... .. .....:=..   :@@#**#: .-=---:=. .@*.:.    .::@*. .-====--. =*+*@@:   #: :..::.....:=#=*#++##-::...::##*++..+...  @@##*+..:-:----.:---::::::..:.::.:::::..::-: :--.  :.::::-::::::::::.. 
 // -:-..:..::::::-:.:::....:..::::...::::::::::::::::::..--:--+- .:#+....::::::-+=+#*-=*--#+*+*=+::::::.:::*%.  *@**##+  :-:--. :......:-=+:... .:  -:-::..=+*%@@   +* -*=*:::-:-:------:-:::::::::-:..+=+.  #*-=  *@#**+. ---::::.:=-::::::.::::::::::..:-:-..--==-  ::-..::::..-:::::: 
 // ..::--...:.:..--.:....:::::::..:::::.::-:-:.:::---::.----=*-.   .--:::.::---:-=:::::---::::::::-::::..:.    .: @@####  ::-.   ...:.#%@@*:.::.   . --:. =*#%@=  .*:  *+-.:::-----::::-------=++*++:::....#+ .:.-  +@%**+ .:-----..--::::::..::::.::::.::::: :+=-=++*   .-.:::::-:::::: 
 // ..::..:-... .. :-..::..:::::.:::..:::::::::::::::::.----=+. -@@:. ..::::::::...::::::.:::..:----:::::::...-+@:  #@#*##. .-.@@%.....  :: ..::.-@@* ::..+*=*@- .%- .....::----::----=-:--:-====-::------:-*#+-+*:== =@#**=.:-:::-:.--::::::....::::::..:::-. =-::::-=*=   :..::::::::.: 
 // :.....-. -:-::-..: .::..::::::.:..::::::-:.::::::::.=--=+- .   -#+:.::---=--:::..::::--::-::::::-----:::==#.     @@#*%@. :   -==:..      .::. :.  :. =++%@:=@#  -:.::-:-:::::-:-:-::---::::::----=-----:.+-+ -=..  *@#**:.::-:--.:-::::.:.::::::::..:::-- :+==-=+++*#@@   .-..::::::- 
 // -..:..-.-=::...  -..:::.::.::.:::.::::::-.::-:-:::.-=--+-.:*+:...+=:---::..:::-----::::::---::-:-::.::::.:. .-@*  +@#*%@ . :@-..-+*:*@@+  :::.@-. . -++#@#     ==*:::-:---------:----:-------:---------::.. :.+*=. .@*+*=.:--:--:.-::::::.::.:::::.:::::: +%**********%@@. ::...::::: 
 // :..::..   =.... .%- :.:..:.:..:...::::.-::::::---.:=-=== ....=::..:::::--------:::--:---::-----:-:----:::.....     *@++%#   -=.... :@@%@% .:..  . ..++*%@   #.%#*..:---------::-----:--:::-----:------=---:*%+ :=-  @@*++..:-::-:.:::::::.::.::::..::-:-  @%************%@+ :-:.-:.:. 
 // ...::--:....  -=-#* .:::.::.:.:::.:::::::.::-::::.----+-==-:..::::-:----:::::::::::::::::::::::::::::---:-::::.:+*- @%**@=  ...:::  %@@%  ::::::...==+%@  +*:   ..:------:-:----:----::--:--:-------------.==*:-#*.  @#+*-.--::-:.::.:::..:::..:: :::::- .@#*************@- :.::::::: 
 // .. .   .:...-++=-#@  ::::.::::....:::::::.-:::::::-==-+..::+=::--::---:-:-::-::::::::::-:::::::::::-:::-::.::::..    @#*#@.  :::::.  -@+ .:::::. .:+++*+ -:    .--:-:-:-::-:::-:-:::::-:::::-:--------=-=-::*-*.:=*= @#**- --:-::..:::::..::::::.:::---=.*@@@@@@@@@@@#*++%. -: .:.:=. 
 // :-+:  +-::-++=--:+@. ::.::.:.::::.::::.::::-:--:::---=+.:. ..:::::------:-::::::::::::::::::::::::::-::-:--::.:..:-- #@**@+ . .::::.     .:::. .-.===*@=    ++:.------:----:--:::::---::::----:---------=--.: ..---. %#++= :::-::..::::::..::::..::--:.              :*=+#. :**  ..:. 
 // :   =%*==+*++==+=+@= ::::.:.:..::..:::.::::::---::==-+=.:##+:-::-:---:-:-:::::::::::::::::::::::::::::::::-.::...=.  .@*+#% :-: ..::::.::::.  :-: +++*@ *@= *#=.-:--:-:-:-:-::::--:.      :-------------=--:++*:-:-. #@++= ::::::..::::..:..::..:::-::=. :*=-::...-+ :#++%- .=+*: ::: 
 // . .@@%@@@@%%#*++==+: ..::::..:::...::::::::--:-:::==-*- .  ..::::--::--:::-:::::-:::..   .:::::::::::-:-:::-:..       @%*#@  :--:.    .    .----:.+=+*@ -=: ** .-=----------:-:::.  :*%@@= :-:----------=--.==*..-=. *%++=.::-::-.:::::...::.:.:::--::-. :*-:=*#@#=: -%++%= :+==#-  : 
 //  @@*+#        ...... .::::::.:::::..::.:::::::::.:=--*=.=+=:--:::-------:::::::-::..:--++  ::.::::::::::-:-:::-@@@+@@.*%+#@: ::::--======+=--:::..+=+@@   .   ..-------::---:::-:.:%@@+-*@:::---:----------:+++:*%=. #*++-.---:::.::.::::.....::::::::-. :+*@-=#*%#+ =%++%+ .---=+*   
 //  @*++## +=--------=-..::::.::.::::..::::::::::--::==-+= -==:=---:-:-:----:-:::::::.:-:::@@. ::::::::.::-::::-...      =@@%@= ::-----:.....:-----::*#%@% -=::::::=----------:::::. .:.=%=-=.----------------..   ==+: @#++:.::::::.::.::.:..::.::::::::-  -+@=+#::%-+ =%++%# .==-=-*#  
 //  @*++*@ =----------=...:::::::.:.::.:::::.:-::--::-==+* .   ..:::-----::-:::::::...::::.--: ::::.::::::-::-::::...-=- :#++=  ....    .=. .   ...    .:- ....:.. -+=---::-:-::..:*#*--%*=..:-:-------------::#++==-.  @*+=.:-:::::.:::::..:....:::-:-:::..-+*%+=+@+:. -%+*%%  ==-=--+% 
 //  %#++*@ -=--------=+: .:::::::.:::.::::::::::::::.-=--*:.+##-:::--------:::::::..=++:-==:. .::::::.:::::--::::..:.... -* -..-..::..-:..-:.:-..::..  ::  ::......  =+-----:..:+**-...   .:-----------------::*+..--. %%++-.::-:::.:::.:...::::::::::::-:..=*==+*+++#@ =%+*%%  +===-==+ 
 //  #%+++@ .+-----=---+- .::.::.::..::..:::::.::----.:==-+- .  .:::-::------:::..-=+=...    .::::::::::::-.::::::......  ** +:.....:..::::::..:...:.:::.--..:::.::..  :==-:.:+**-..::----:-:--:--:-:---------:#*=::=- .@+=+:::-::::.:::.:..:.::.:::::::.... :-.. .    : :%**%@  +++===== 
 //  -@+++@- =--------:=+ .::::::::..:::.::.:-::::::::.-===# .:==-::--------::::===....::::::::::::::::::.::::-::.::..:--.@- -  ... ...  ..  ..  ...  ..  :.   .  ...    .:+*=:....................::--------::...:+=  *@++=.:-::::..:..::.:..::..:::::::::  =#+****##%%%%#++#@  ======== 
 //  .@*++%* +----------*. .:...::::..:..:::.--.::-:::::=-:*+:-::..::::::-:::===:::...:............:-:----:::::.:......  .@:.-.. .  .   .......... .....::::::::::.:.:::.:+-.:---======+=++++++++***:------- :*=::=+: :@++=..::::::.:::.:....:.::::::.:::-...@@@@@%%####*##++#@  ======-= 
 //   @*++#% -:-:==::---*. .::.:::::...:..::.:-..::::::.-==-%: ..:-:::::::-==-::::::----=+*#***#*#+           ........... .:::---==---==--=--=------=-----:------=----:--- :.+*++++**++++*++++++++*+.--=--:.-*#::#*  :@+++:.:-::::.:::.::...:::.:..:::::::.. ::....::::= .#++#@  =*====== 
 //   @#++*@ =---===-:--*- .::::::::....::::::--.::::::..-=-=#..=#-:..::=+-:::-:-==++***-.:        +++*-......       .... ==:+======+=++++++++++=======++**++++****##**+=+::.=+===---.-+-:.-+: =: ++.=----:#++ .+=. :@*++-.:-::::.:-::.::..::..:::::::::.:...**+++++++*@ =%++*@  +======= 
 //   %#++*@ -==++=+++--++ .::::::::::.....:.::-:.:::::::.===++    .:##*:::--====+--.   .. = := ++ .... :+=++***+++*++===- -:=-=:---=-=-====-----------=..::-=:.       --=.:.++==+-==:=+-+=---+*=:*=:-------.. ::. :@#**-.::-:::..:::::::.:...:.:.::::::.:...++-:::::-.- =%++*@  +====+== 
 //   +%*++@. -----::--:=* .:::::.:.::.....:::---:.::::::..===+%: *@=.....        .-=***--=+=====-===-=..=:::....:..  ...:.-:--  :.            :-------.        . . :. =-+ -.**+++++=-=+=+-=+-==-=+-:-----::..--. +@#+=-.:::-::..-:::..:....:..::::.::::.:...*+=@%@%%@#* =%++*@  ++====== 
 // . :@*++%+ +----------#. .:.:::::.::....:..:::-..::::::..=+==#+        :+@@@@%#=.    .-----===+====+. -::::-::::--:::::.=:=-=*+++++*%@@@@@@#*=----:-+*#@%%@@%@%%%%%*=-+ -.:...::::::::::::--::::::::.:: .   .:@@#++. ::::::..-:::::::.......:.:.:::::.:..:*+-*=++*+=# =%++*@  =+====== 
 // .  @*++#% +=---------*- .::.....:.:.::.::::::::..::::::. ===:-  :#@@@@@*:        .:::             ...=--=--------:--:: .---.  - -*          -------                :-+ .                               .     :=+=. .. .....::::..::..:::.::..::..:::....:*+=======:+ =%++*@  +++===== 
 // :  %#++*@ ==-------::*+ .:::::::.......:::::::::...:::.     +@@@@=         ...:.......:.......                    .:-: .-==+*++===+*#%%##%#*+-=====+******######**+=-= #@@@@@@@@@@@@@@@@@@@@@@@@@@@%@%%@@@@:-:.-:-====--=---::::::........::......::.:.:=#*****###%@ =%++*@  *======= 
 // -  #%++*@ :=--------:+* ..:.....:..:....::::::::-..    :@@@@+     ..-====++===+=++*++++**#####%%%%@@@%#%@@%##*#*=-:.  .:.=  .-=--==--  +:    ......  :===------:---=-+ =-----::::.:...   ...        .....:-=-::. .            .. ....:..::...:::::::..               =#++*@ .+======= 
 // =. =@+++@. +-========+@:.::.::...........::::...    *@@@*     :*%##****+++++=====-===---:::::::::.......  .....--+==++++:= @         @ + %=*++++**#:#::             .+ .                             ...... .::..::::-:::::::..............:::::::::::. %@@@@@@@@@@@@%*++#@ :+======= 
 // +. .@+++#- .             ...:...:.:.::.. ..:.    #@@%:     .::..                                                        .:=+    .    ..+ @.*::::.:: =.:+@@@@@@@@@@@@.+ =:========-==================---:---=======-====+=+=--=++=+==--::::---::......:  #@+++++++++++++++#@ -+======= 
 // +-  @*+++%*=--+*#@@@@@@* ....:...::::::--.    #@@#:     .........::::---------==--==========++========+===+++*+*+======-.:-@@@@@@@@@@@-- @+%@%%%@@@@+...*         .# = --------:----:::::::::--:--::::::...-=+===--::::::-:-::::::..::::-:::::--::::::  @#+++++++++++++++%% ++======= 
 // +-  ##+++**#%%##**++++@# ...:.::.:....:.   +@@%-    ..::::::::::::::.........:.::...........                     ........::#@@@@@@@@@*=: @:-....:..:-. =% ..:--.  =% -.-::-::--:::::::::::-:::::...:.:::::.:----------::-:::::::.:::..:..     .:---::. :@*+++++++++++++++%* ========= 
 // =+  +%+++++++++++*****%@  :..::::::::.   %@@+.    ...:......:---:::::::::::::.......::::.:::::..:..:::::.:::::::::::-:::.-.+          #. @.#######% +. =# ..      ## -.--==--::---=---=-===-=--------=-:----===----::::-:::::.::..:.::..:=++++=:::=--. *%**+++++++++++++*@: **======= 
 // ==. .@+++++*******++++#@. .-:::::..   .@@@-    .-:::--:::-----:::::-----=---::::---::::::::::::::::::::::::..:..        .-:%          @. @          #. ++=::-- .%#%# :.     :-:::...:...::::::::......::::.:::---::---::.::::::::::::----=-:---::.... .#*++*++++++++++++#@ .+======== 
 // =+=  @#+****+++++++***#@=  .:::::.  :@@#-   .===+=++=======--.::::...... ...............         .             .::::.....-  @@@@@@@@@% :. @@@@@@@@@@ : *@@@@=**@@#%:.:.                                                                               =+===++++++++++++*@+ +++====--= 
 // .::. *@@@@@@@%#####***+*+.   .:.  :@@*-   -*+=:.                                                                         :-.          :::.          .-:.     ..    .:. +-:-==------===========++++++****#########%%@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#****++***#%@@@@@* .-...::-=== 
 // **++=.       .=******++==:       #@#-.  .                      ....::::--===+**##%@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@:.=-----------=-----:------=--=-==---========.:@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%#######%@@@@@@#+.     .=+***####*#* 
 // ####%%%%%%##*=:..      .+@@@@@@@@@=-  :=..=@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@%%%%########******##%%@@@@@@@@@@@@@@@@@= ...                                          +**%@@@@@@@@@@@@@@@@@@@%###**#***+++#@@@@@@@@@@@@@@@@@@@@@@@@%%%%%#####*****#%@@@%+       .-+*######******++** 
 // ***********######%%%%#*=:     .=+-.  =--*@@@@@@@@@@@@@@@@@@@@@@@@#*+===++***++********##%@@@@@@@@@@@@@@@#=-                      .......................... .                               -=*%@@@@@@@%%##@+ ..                      ....:..:.-:-=*:   .::=*#########****+++***+==+* 
 // ++****+++=++**########%%%@@@+   .   --=+*+--::.             ::  *@%**#*****#%%@@@@@@@@@@@#+.                   ....  .     ..:-:==:=+*+%%*%%%%%%%%%#@@#%%@@@@%@@@@@@@@@@@@@@@@@@@@@@@@*#-.               .=*  -::-==*******#++*+*+********++****+*+      @@@@%#########**=:*::::.**.: 
 // : .. :-@@=%. :==++++***#%@%  .:.-=. .:=====++++**######*+=-::=    @##%@@@@@@@%*=                    .....   . .:.=*+%*@@@@@@@###*@##*++=+++:#==*=+=+++=*:+=-*+-+=+==+*+-=-*+:*-=--**=###%#@@@@@@@@@@@@%+-                ..:--=+=-..             -  ....   @@*++==----:: .%@#@ #%*=@@ 
 // :@=%%   -. @@+      ..:-=   ..:..:=+..:......::.:-::-.:::..::-:   @@%+.                          ...==*%##@@#@%@#*#**++-=--=*:+==:*+==+=+-=:*+-+=*--+=.+===-=+-+====+--+-+=-+==+++=++==+-=+==-+==-=++=**#%@@@@@@@@@%#+=:.              .....::=#-....::...  .:...:::::::+@#:#@:@@@@@- 
 // .@-#@* .*@.-. :*@@@+. -*   .::::...:-                ....:::-==                      .:===+#@@%%%%%##**++-=-+==-==+-==-=+++===++=#:.+=====*=+:====+==+*===-+=-=:+=+-++++-=-+==+==-+=-=+==-+--+===+:===--==:==++++=++*+#%%@@@@@@@@@%#%#***##%%@@: ..........   +=---::::.  =@.    ==   
 //  @  @-%@@%@*=@@%    @@:  ..:...:::...*@@@@@@@@%=   ...:::...::-.... .         .-+#@@@@@#*#*#---#-==+=-*==++++=*===:+-=+=-.+-:=:-=:-:+--=+-=-++===-=-=+.=+-+-===+-=*==:+==+===+=-*=+=+=-=+=+++*=++*=*==+++++-==--====+-====+==+==+*****+*+===.             ...  ..........           : 
 // =@#@      .:- -@@@+@@   ....:.::.:..  :##%%%@@@%%+..:.:. :-:..:         .+@@@@@%#++=+-:+-=+=-+=-=*=+===+=+==+==-++-+==--++=++=+=*+++==++#:+-+*+==+=+-=+-++=++===+==:+--=+======+==-===++===-===+==+*==+-=+==+++*+==+====+======+++=====+++**#%@@@@@@@-             ......  @@@@@@@% . 
 //      .  =-  ..   @%   ...........::.:.         ==+-.::..         =@@@@@@@##++=-+-+:====+==:*=+=-=--==*=-=-.=+-=:-=-=+--*+*+===-+==-=-=:-::+-:=.:--+-+-=-:==-:==+:==-===-+:=+=-=+-+=-=-====:-==--==--=+=+=====+=++++=++=+===+===-========-=====+++++*#@@@@@@@@@:            %+:=   @   
 // ....... @@ @*...     .............:...::=* .:. .::.       :+@@@@%#*-=--+=+==---+==++=++=====-===++=-+:=-++:+:--+===-:==-:::-=-+-:--=:-==+===-+==++=-===+*==+++=+++++=+*+++++=+=*=+++++=+=*++++=++++==-==++=+===========+=======++=+=++=+=++++==-+=+===+====++*@@@@@#=+%@@@+=: +*+     
 // .......       ..............:............:. .      -%@@@@%##=+-=-+=+=+*--:+=*%:-====*-:*-==+:==--.#-:*-+=.==*-=---+*.=+==++-+===+=+**+++++*++++++*++*+++*++=++++=*+*++=+-++*+*=++++++++*=++++++**+*+#*#+++=*+*++**=**+++*=+++++=+++=+=*+==+===++==+====+=+==-+++=**%%*  :*@@@@%#@=@@@ 
 // ......................................       -*@@@@%==-=-+====+*=====-==**===--*===--=-=:--=-==---==:=+==+=-==+=++=+++++*=*++*=++*++++++=+=+++++==++++=-=++=++=+===+++=++++==++++==++++*++=++=+========-*=+==++=+++==++++**+*++**+***=+**+*=#*=*+*==+++=+=++=======-=#%%:   --#@*%@** 
 //            .  . ...............        =%@@@@#*==+-==++==-+==--===*-=+++:-:-=:-.--+-===++===+++*=+*=+*=*=*=#+=++++++**+++-+==+++++=++=++++=+=#=-**++-+#+#-+#=++*=*+=*-*#--=*+-+--=#*=+=====+*:*=%*+*++*=+==++==+==**+==+-===-==-*+==**+=*-++#+-*+#+*#=#**=*#+**++++***:*#@@-  . @#:%# 
 // -:::........... ..........       -#@@@@%*=--====-=+=+-=-+=+==+++:-=:==-===+-+=+==+=+=*++++*+++=++***+*=+=++=-=*+==++.=+++++++++=++*=**:=+=*#==*=*+---=-=+==:====-++=--+===+-=--+*-:-===+-=+=-=+-=+-=-++==--+++*++==+-+=+=*=+++==-===:-=.==--+-+==:-----=+++-=*=*+##++#**+*#@@%   .%%# 
 // ::.:.-:::--------:.        .@@@@@%#+=====+==+=-==+=:=+*=*+=+===-=+++=+=+=+=+++=+**==++==+*++++=+=+--:*+=*-=**#+.=+*:==**#.--=+-+*:=+..-+=*.::.::-.=#:-#+..-+#----=.--*-+:- ++.==:*+-:==.-+..=%-. -==-=.=+*+==.:-+=.=:*=++:=--:**+--+%=+*++++---==**=-+-:=::-.-+.  +.++.-= -=+#@@= . + 
 // .:.:::..........   -:.+@@@@%*+-=+====+===+=--++=+=++***+*=-+-=+++++==+++=+=+=++=+--++-+*--=++==+=-+++-==*-=+===.=-+-=--.---=:--:=---=:+--:=:+=+:-==---.-==-=---::=:=-:---=-=:=-:-..=+--=-=*=--==:===.=::::--:=-=:=-*=-:=-==-+---==::-=--=-===+--=-==+=+--*=*+---+=-==-:===-:--:-+-    
 // .:...            *@@@@#++=-======-===-+==++-====-+++=-===++*=*+==+-++*-=*#-++:+*-*#=.#*=++=--:=-=-#.:#. - *-:+-:*--=+== =+:-=:==-:-.:**.:..=+=::+=-=+:==-.==. =***--=.*:-:=*+--:=:%=.:.=+.- -:-*+=:-=-=#*=-==#.=--.--+--+-=-==:-..+*=--=-=--=+===-++:-+++----+*+==+=-=+=-*.+.=-++=+   
 //         :*@@@@@@@#+=-=+=+==+=+===+++=====-=+++++==-=+==-+=-==---+++===-==-==++=--:.:--=+.-==:=.+=-= :*#:=--::-=-:*:-.:=-+=:::--::::==:.-=*=---=+=-:---=:.---+=-:=:--+*--=-:-::+-+::*-==--=+=+-=-==+-:--:.:-=.--+-=+---------====++=-:==+====---=--=--:--=++-=-=+:=:---+====+=-=+-+%@: 
 //  =@@@@@@@#*+========+=+=+=====-+=+-==*++#==*==-=++#.=-+=++=-+#**=.-=-*+.:-% :=-.--@-= :+::.::.::= -+:  =--*-+.:.: :--++=::-+#-+:++*::=*=-=+-.+-.:--:*.:*%-==--:-:-=::.-+====*=:=-+-:-:==-+--=:=-.:.++=-#-*::=-====+*+-:=:=+=:-+-+-+:-=:+=+=====-=-:+=++-::-:*++--+=*+:*=-=+=+-++=+=+# 
 // @@#%%#+= :===-==+==========+++=-=++===--.:=---=+=+-+*:=+==--#--+.===-.*++=..=-=-=.---.--=-+:+-*=-=+-++-+==-..===+*=+=.-.=+-===--:=-:+-=-=--==-=-=-=::-=:---=:==---=:=---.:-=::++::::=--:-::=-=-====---:-:--=-+---:.-:-=--=-=--:=::++===-:=--==+=--=-===+=-++-:+=+::+=+:=++--*+:-==+-# 
 //  :**--:=**==+=*+==-+=++-+*======-+.-==*+#*=#+:-=.*.:=-:-..+. .-.-:.:: :: :::=-+ .==-.=+-.-=++::=-.-:::=-.::-=++.=-:---=--=.=.=-*:+:++--:::=..---=-==++=.:=-==+.:::=.-=-++*.:-=..==*=--#=++==-=-+--::==.--=-==-+=-==--===::-=-==-*+-+.==:-++-.:=--+-=--:=-==:=*+.=+*.:++-:.--:+=+--+-* 
 // @@-: -+*+==+=+=-+==+:===+::+-:=+=:+*==.---.=.=*-..-=-::.+-.=.-=----=-++-++-++.-+:..-:.-+::+.--=+:#=:+.+:*:===:- :=:-.+==:-==:.-:=--==-+-==-=*+=-==---=---=:.-+-=--=--=--:-=-====+-:==--.=----+==--==-=-==-+=------====:=-=--:=+-:=--=--+-+==+==:==+*:=-*--====:=+-=*==:=+===-+=-==-=- 
 // --.=*+#::*--+=.+:-.---*:=.*-=+::==:.-:+:* -:-=.=++-+.:*+:--++.:+ *--==-+:=..:+-=*+-+#.:=+:.*# *=:.=:-+= :.#:--++====+:--:*==-+:---:--==+-%=.-.=-:=--::===:=-::+=++--:::-=+-..=::-::---=.-=+:-:::-:==--==+:.:--==+:-=::-*=.=+==:--*-=+=+=-+----:==-=-:=*-===-:-+*.=+:+*+=--+=+:+==*:-* 
 //  .-+=--=.-==:+-=:+-=*..+*+-.-:---.=*::.-*.*.---:-::=*+.:#..===++*=.-:+-:.:=#-.=  =::.=*--# -.: -+=:.*:-++=+:-=:=:=---=-+==:+*-==+:---=.--+-=-=:=-=-=-=*---====::--=++=--:--=-=+-==+-===-=--+=-:=--==+-=--=-=+=---=---+-.-=+:-=-++::=:--+--=-+==*:++=*-:=+*--+=+-+++:-+:*:#===+===-**. 
 // .*-::==::++--:+=:+:===+  =-.+-+= - .:+=: :. =*:= -*  =*.-===::::*.=-+.==+=:.:=:*:=:-=:-+ -*:.:++ ==*.+=.:.:-----.=-==--:--.-.:-+-=-=-=+.--:::*-=---------==--=-=-:.::=:=+-==-=:==.-::=--=-:--::=-=+.---.--+---::--+===++===--=*=:==-==+=====+-+.-=::=+=+:=+=:.==:.==+-.*++.----+-==-- 
 // -*-.-=-:*+:- +.:-+-=-: -+: :..+.-=.-+==+:*:+.=-.+--:*: -+-:+.+: .+-=-.+:- =:+-:=::*-.:::-+.*=++--* -=-:=*= ==-:*--*:+--*::=-==--=+-=++:===-:-=:++====:--=+:.-:---+===-:-.:-==+-:.==+=:=--::--=:---.--=-=--===:+++--==-..=+*-=-:.=--+--=.--=::=#-+*-::-*-++-:=+*-+#=-*=#-:==+-+-+=-..= 
 // -=:-*-=+ : :=-+==... :#-.+=-.+-.-+=*  :: +-=-=-:::--=.==+--=:.-+-:::=.:=+:=-:-+=-=+.=+-..:.+.-::-:--:-==--+=-*:---:--:-:=+-:+=::=:.- -:=-*--=:--.:+--:-:..+:-=-:::=-=:---==:::====.:---=====-=--:--++=.==:.-===:.-=..-=*+.-:=:=*=+--=-+==+==-=:-=:+*===-+-=-=-==-.+-+- ==-:=#=:+=-=#= 
 // ::-=.:. :#-+..:=-.=:.=::= -=-:-::::.===+.:::-=--=.+.-:+ :-..++= --+ +-:+==--..:+ .-=-.==*==-:=-==.::= :-+.-:.-=-:*.++-==--:==:==:-*=+---:-::::-*=.--+==+---=======.=-:+=--:=-==:=:==--::==-=:==-====.=::-++==-=+---+=-=--:=-=-==-+==.+--*:+*=.+==-=-.+#:-=+*==+:*:..- .%:*=#::-:+==+= 
 // :.:-..+:=---..-  =--:--:-:-.-:.=::-:-=-=:.+=--:=:-=+..-=+..+:.: =--+:--::.+:-- -+=-=.=*:--++===-=-++:=----+=#=:+==::-=-=:-==:-:==:==:::+-+.=+-.=.=--:==:=:--:==-:--+-=-:-+=-=-=--:===*--::=+::=-=--:-:==-::.:-=:++--==--::+=++--.-+:++===---*==-*::===--=--.  ::=-++*@%+:==:=*.+=-=:- 
 // :==+.=::-.:==---+.:++.:--+---.*-* .--  :==-.- :*:------.+-+*:+:+# .-::--=:-:-=*#:::==+. :-* . : +.-+.+-+:-:..=--::=+=.-==::-:.*:.-.--=--:::++-=:-*= -=:-+=+--.:++:.=.::=*:-- :====*..-=+==- +*-=---+=-=---=*.-=+---=+====++--::-==-:+===--=-:-=:-+*+=-=*=.#@@@@+-:+%*%@#-:-+=:-**==.= 
 // -*...:..-:: ::-+.:=- --.--:-=.-.::+.:++.---.++-:-=:::*-=.-: :-.::-#-=.-+.==:+-..+-+=:::=::-====-:*.-+:+=-= =*---*:-=:+::.# :+=+:=*::--=-++:-.-%=-:+-+=:+.-+:+==:.=+=:=+:--.++=::.:--==:.=-+-::-::::=.:-:*--++-+-.----=:--=-:-=-*=+:-=-=:===+=--==-=--. ++-  * . .-:.=  %--:-===- ==#+ 
 // =.:+==.:--=-:*=..=:..-+.:.=.-+.-=+:-=:::-:.+:=.: .-+.-.+=-.====--- =.:.:--.+= =..= =-:=#=:+::-=+*.-+=--:+.*-  --:* . =.*--*+=-:==.-+:-=.:+-=-- .--:+=. -+-+ :-==:=-.=.::.:==..=+=- --.-:-::::++=--+:==-.=-- -----+:+= =-:++-=.-+=*:*:-:=---+.-+=:::-=+*---*  ****- :--. .+=*==::=:::* 
 // =:::::-:=::-:- :-==+.+=-=:-:-+-- -:=-::=-+- :==:++-.-:*-.---=:-=-=-=:%.:+-=.-+-:*=.:+-- =.:+:=.-.= -:-- :-:=+:*=:-**=++-==---.+=-=:-+--=:==:.=-+*-:=-==:::-**-:.-+=:+--===:-:=::=:=+:-=+=-===----++-+=---:+=+--.-==+-::-=--::=--:..-:++-=*-=+-=+=+=++=-.:#=+-: -%#-++=@*+*+-*==+*=#+: 
 // ..=::.--=.-. +..-.--.+ -- +: :.-:* .-.-+: .=-+==::+.:= .--:-: =-:: =.:=.:.:+ :...:# .::++ =:.= +++-+ -:=+.-=.= ---::==.+:=:+:--:-*-+-:.*+..%..*=.-:-=:=-=+..++:-:.-::+* := =+==::=:++#..:-:+=+.-:== =.*:+::+::-===-.-=:*=:#=#+-==+=*=#+::=:-.:*==-*-.:#=+:=:+**-:--+:*+*: +-:++-+..*+ 
 // :=:.=*- =.--:-@==:- .=.-.+--..:::-=::=.-.==-=:::.=.*=::#-..=:*=- -:=.=====-:.*++-:.-*----:#: :-::- =:++ -+ -===-=:+:====..-=---==: ::=.+=%-=+#**=+%=%*=*=*+=:-=--=-=-.-==-=--.-+=:+-.=*-=++-.-.*::===::--=.-+-++:-:=-+-:=:-- --*:+-*-:+=-+-+=+=.=:-=-=-:::==---+*+-==-.=-*-------=++: 
 // --.:= .#.:-=+= %--=+:-- + :*.:*.: ::: .-.:..= -: =.-.--:=.*- .: =*=+= .* .**:. +*-*=:==.*====*-*---::-==#-*- .=-:.=.::-=+#= #=:-*@%@*%@+:-:+:   = .   :::-.%@=@%%@%**%-:-=+-=-==-=-=- ..=  *-*-==:-. *+=+::--+-.+-=-==.+--*.---=.-:--..:::+*:+-*: #*##+=*:+=.-*#.:---+===-=##=+=+:==- 
 // .=--.-.: ==.::  :+#*#%%##=+ ++: *=:= +:--.+..:=+*::-=.+:-=.=---::::.+.--.- -==-=.=:=-:+:-:--...#.-#*::=-.=+:++=:=:-.+-*=*####*+=.                                    .*=%#+---.-==+:++#==::--.-:=.=+=-:-=+==-.-==+=-.--:-::-*=.-=*+---=#*=---=+:.::-..::---++===--=-++====----+*=--=+ 
 //  --:-.-:=- -=: .          -  :.:  ==#*+=:==:+ :  +: :.*..:= -+*+*.#: :+*+-*:..:. -: # -* ::=+#::=:: .#-=:  *-=-##*+@@+@:           .+*#%@@@@@@@@@@@@@@@@@%@@@%%+.         .==@%=.: :..  *+*+=.+=.*-==---..++-==-- -=:#.=#-:: +##-.+--++. . :+=.*#+*++-#==:*%=+ *=:=.*:#+#:+@%**+@@@+= 
 // -+:-+:=:.+.+:- :---@@@@@@*.=-= .-.=.. .:-==.::-=--..=-::=+=:-::- --.----=. -+==*:-= =.. -=-=.:=::::+.--:===:+:-#+-        ..#@@@@@@@@@@@%%#**+==-.    .+#%%@@@@@@@@@%=-.      . +@+*-+#+=-::-:++.:=-+.:=+=--=::=+-+--==-+=+=*.-::+=:==*+=+*+=+:+::=.-+:-=+.-::=-:*--++**#=:-      . . 
 // --.- --=.=-=+: ** .      .  :-+-#: :=.%.::.==*==..+=- +*  .-=-..*=- -+: -+== .:: +=*.-+:=*:- *=..*:+: - =+-:+=:      .#@@@@@@@+  ...:-:.           @@@:...       ..*@@@@.@** .     -+.=..#:--*::-=..-**= .+::-*#-.*-.:.:-. :.--+:.=:%=..:+. ==. -=#+=++-:--#:***+%=%*:        .       
 // =.==:-:::.: :*:: -=-++=-*-+*::+ :-:=.= . =: =  =:+- -: :::=.-=-=: +=-+ +.::=+:.=-:-..-=-+.-= :.+-= :++-*-==.     .=@@@@@+.     @@@*-:. .@@@@@@@@@@@@@-.::::-:.   ..... :@@@=:.   .: .+--.====-.-==--=:=:=*=::=- .-:-=+#=-*=+:-:*-:- --+===-+=-%===:-=+=-+=-+=.: :+=.  .:::          : 
 // : +....+-.-::  =::=: :+*.-:::.=:.+=:*: *# :*=:..: +=-# .:=: -+ ...+= :::##  -=-.=-*=-*: ..+ :#..: #. -:- .. +  +*@@@+  ..:-: #@   ..  *@             ..::.... @@@.:-.:-..  @@@@.==  - #+-=.+:-=:--=*- -+ .--+ -+##.=+:-.+==:=-++==++=:-#=.- =*: *- -:: -+-#...@=+:  :-::   #@@@@@@%## 
 // -=:.=-.-:=::=-=:..- .= -+.:.:::::-..--+.: -...* -::-= ==:-.+--.-*.:=..=.:.:::*:--: -::+. .=:.== -:=..+. ..-.:-+@@-  ..:::--:.@@+@@@  @# @@@.::.  .. ....     @@- .:::::.::...@#  :@.  .:-::::-+-=-==::-:+*:-====::-=:+:=-:-+*+-:-==-=--:-=+=:--+::*==+==:--:*=-+= ::=*  *@@*=-=----=- 
 // .:-# =*:=-=:.==.::-=.--  ::=--+::  =:.::=+=.+=+-+.=:.=:: .* -+.:.+=..-=::--%- =-.:.=-:-#+*:=:-+#-+=-:-::.::..%@ ..::-::.:..   @@@ *@@@-   -   .@@..@@  .@@@.+@+ .::-::::=::. @:.#:+ --=--=-.*#:.:--=*%#.:-=-=+-.-.==:===.=*:. ---+=--:++-*=::: +=:.-.+++=:++.+-:.:-:#  @%------==-=-- 
 //  ::.::  :=-: -:=::.+=. :-==::-..:-+.::--  =--  = .:+- .*----:-::+ ==.--.:-.-:::-.=.-.=. - +-:.:.-: :--.-=+..@@..----:.:::..@@... . @*  #@@ @@%@.%@:*# @@- .@@.  .:-:::::.:. =@.:.: -==. -=-+-..+==.:-: -+:--=+.-=+::==.==:.+:+=::-.-.*===--+:+==-#+-+--::+--=-=+:--@  *#=-=---------- 
 // +.::-:#::=:..+..=-..+.-+  : := :=:.:=+: -=. =+:+ ..-: +: = .=.:.=-=-*..--: -:#-:+..+=--:=+::-.:*:+#..*+=:= .@: ..-:---:.. @% ..  *@+  @@   @@. .@-@@@@@.  *@  +@--::::.....@@. . ==-*=:--:=:----:+=:.====:+=-=-*--+#=.+*=:+.--:--.+%..-::..++*-:- =+-:--::..*--*.-++  %=--=--=------- 
 // =.:-*=..:-.+:---:+:::. --::=..++.-...=--*.-....-+* .-+=.=..:.-*:--..-.--:+-=+ -= ..+:=:=::::=.-:-..:=:--:+  #@  .::-:::-.@@   .*@@= .@@*  +@  @@: .  %@@@@@@@@+......  .@@@.  +=:++=.:--.+*.=*.*::++-.-=.--::==:==.:::..=:-+=:+====-.++:-#-.--=-:-=:=-+--**-::+:-=#  :#----::-::::::: 
 //  .:   .:.     :.  .+=+#==-:==::  +++ : -.::*:.*  -. -  .::*::.*.=- :-=:=:-:  #  +=-  -  - .#:.+.:.=**+*+--:  -@@.  ..:--..@@@@@@@ ..@.%@@@@ .@@-..... .=   -      .-@@@@     =.*@*=:-*===:-* ==.=::=.:+-*==----:.:-+.:+==*-:==--*::--*=+++=---=#+-=--=-#-.-+*----=#  ==--::..  .:..-: 
 // .....:....:....... . .: -=: -..+= .-.-=::.--.-++*: +:.--:-:*..:  :.-+.:=- .= -=:- .=:=:=--.--+.==*: ..+++===   .@@@#.       ... ..-:...   ...........       .%@@@@@.     =+%+@%:..:-: :..-:.= =* :=+=:=..-==.-:+=.-+-:--:-.==::..=*+= : -=-+*--::-:=.=====:*===.-=+  +---.@@#@@@%-@%* 
 // ......       ..:::... :- .+.-=+- =-  = .=.: -=. : -#==:: ..::=-=+:=:- -.:-.++ :=:++-:=+ +:.*::- - -=+ - *+***=     .@@@@@@%+-.....    ..... ...:-+*%@@@@@@@@+       .-@%@*##-.. *::+===:-=.:++. :+ ..==+:+=-:+==+++:-===:-:=.:=-=-::====*: .::=-+=-+--.:+.:-..=:.== .+---.@=.@@%#%@@@ 
 // .......:::::::.   ::.:.:::- .-.:=-:+ .+: +.-:-::--.  ::-+--: *.-:==:= ::-=* :- - :--=. %:::+=:-%:-*..:-+ -+#*@%@#=:         .:+%@@@@@@@@@@@@@@#+-.           :*%@@@@@%%::=--:=-...#.+--* *+.:==++.=#%-- *-.-=+.:-::++:-:==*=:=:.::+==-#-:++-#.-+=-+--.+-:-=-++++-#:  :                
 // ...............:=. .::...=: -  -..:.*:-==+=.::+:-+.+=--:.:=+ +.= =  :*:= .+ .-=- :+ =+. .. :-.:::::=+. .-+.  --=##@@@@@@%*-.                     .-*##@@@@@@@@%*#-+ : :.--. ::*#+:.:===+-:-=:.-:-=+:..-:-:*:-----.*- =+-:-.-===.=+.--:--=:==*==.:=-:=*+=.:=-.= =:#  -:@=@%@+@--@ @@@* 
 // .................-=  :::. :-:-* .*+.:- := .:: -= .-::. +..--:*  +:.=:.:.:: - =-.=: .: - :#*.-=.--.: #*:= : .*: :-. .+= *%#*@@@@@@@@@@@@@@@@@@@@@@@%#*#+=-:=.-- ..:  -:=+ .--.-.- :+:=.: +=.:.--:*.:- -+**..:.=:-*: -.#:.--:--*-#-::::---=.. +-=:+ -+. -+--==:-:--%  =*@ @@@ @@@@@@.@. 
 // ..................-: .::.=----:-:: .:..: =:=-:=- -.=: : :+:::.-+:.--  :-.+::+::=.-+.::+*-:=.  .==:*..- +=+.-.-:. :-  :.-:. .        =..-.  ...  : .- .: :.= : :-.=+ -:. .=..=- + --:-.-  +:===:--=:----::=-::=:++.=*=..:*+=+::.:--.-*+:::=+.=-:-=:==.++==*:-==.:== .=     .:     .:.  
 // .....:...::........=  -:...:-: - :-+-. +:=::=: -= -: *.:* .-.=  :++:+%-: .*  .-.. % % :. = :#-+* --= *-*  -:+:=  =#:=-:*...-=+-.-*:..=.==#=*..#-.=. :=*+#- +=*:+* .:=+#+-*% =:-.#-%:++**#-++-=+=.:+: -:..:+.--#.--=:-*+- .- .-:--:.#. -#.==..:+-+-=----:.-+:--::#  =-:-::-:::--::::-- 
 // -:.................:-  -::. -++-==*..+.-.*:.-== :- =.#.:.-= .::=.- .= .:= -.-:::=:: =::: = .=:. -...-: :+ --:.+.* :-::--+# -.-=:-:+=-..-. ::*--=-.=--.=. =- =-.:==:%=. .=  :.*==-: .:::+.::.-=:..=*:=+.*.-==-::-==:-:..-+*--:++:-+-:+*=:=: :*-:-=:+=..=*:-=::=--#  =-:::--:-----=:-=+ 
 // ::.              ...:. :::::   -    - :+ .. :  +-+= :--:.+=+---   -.= -  +%=:*-=-.---.:: :: - :-:=+=--=.-:# -.:* -  +.:   -+  -=+ .#::-*- = *..=-.-.:: -+=-:  .: *  =:*:=+.# + .% %.:...-*-.::.#=-.=.-++-@##++:=%-:-@#@@=.:%@-++*=#- :+:.:**=.---= .*=#.=::#* :++ .==---::--:----=+=  
 // ..  =: #+ @@@@+@- ..:: :-..+:*. .+=--:=::+.=:+ .- =.: ..:-. -.++-=- -:=:-: . :-. .=..+.=#*-==-+ - :=--..==  *..---=+--..+--.==. :+. .--:- ...- -+:.==+.--%*+#*==+%**=+#-**++*=#+++++=*:#.:- ==:.                                   :+-:==+-::=- :=+::::=::=----*  -+::---------::.... 
 // .  @@+ @@%*@. @@@ ...-  -.:.. :%- :#.. :==:- :+*--:=:.+*+.==.- = :*+.=-+=  .+: -.++..:#=. -. --+.::: -+: :-:--::-#.. ==+..=*:=@@+%@@+******@-+=*=-.+= *=     .:                                  :-=+:=+*+%@@@@@@@@@@@@@@@@@@@@@@@@= -:.-  :+==--==-..:+*..=:::#               ...    
 // .---@@@##%:#*     ...:: --:..=- +:.---.::::.- .: : :-::. :.:.= :--::.::.:++-.+:-: .=+. -.=+-:* ===-:+ :-=+#+:+==-:*:+===::- ..                            .:. .. ==:++*=***#**####%%#@%@@%%%@#@%@@@@%+%#**+%+==-:.--:.    .- . =- :=*-=+*#-:-=-=.-+-++.--==-:.+= :##%%@@%%%%%######## 
 // .              ......:-. -:.  =:* :   =:= -: -=---:-:.. .*:- *-::  .-:=+: +:: # .#: + .@: *#-+: -..:@ .=                       .: =+=-#*##*+#+*#***##+#*##%=##+%@#*%##%@%*@%*#%%%@#%#=*++%*-.@. =.   :+  .. . *.:+ +  .+...... := = -.:...*- +--- .+--++.#*:==%  --.                  
 // ............:.........:: .-:..-..+ --::::=..-+ ::.-::-:-::.=-:.-+:**-.===.--- -.:: ..                    :.-:.=-=**+*#**##%##%***#*+%##+#+%%@%*%#@%##%%#**=%+*%::+-=:.:-  .  -  .   . ::  :: ::::+-:+...=-=::-+:=- .:-+=+=+:-:-=:--**::.=..-:+=:==-:----=:+ -*- .=::::.-@**@@@#@@.... 
 // ...  .......:...:..:...:. --.:.. ::+:* :-.:+..==-...---= =:  :  +                 - -:+*+***###@##@#@#%%#@#%%%%@%#*%@*+@*==@**@-%+*.-.+:#    ....   :       .  % = .=  .-=..=--  -.-+:  -=:+. .--+.. =@-- .=% .-..+#= -+-- -=+-..==. -=*.:=+.   =*:*-.--:.---*  --.::: @@@@@      ::  
 // ...*=...:..............:: .-...:=-:  =.-::: ::.-.-.--:..---::-=*#+*##@@%@@%@@%@%@%*#+##*#-**#.*+=-*--:*-.: := :   .  -. .       : .:  :. ++ +  -:-+.-=--*.*+ *-  -:..-==----.:--== .:-:=.=..:*.:::+:-:: =:-:.+-:=+::..=:: ++--.=+=:==-=:-=:---#+-:---=---+=:=- .=:::::.      :::::..  
 // ... ...........:........-. -:.-.= -#- -.=.:%.==. ::: -::= -#-.-  .==:     :  -   - ::.  :-  :-  .-+   . .-.  +::- *.+  ::. -:#*. =-::-+.. -%-.# =%.#- :..:.:*-*#@::.%-  .. -+:..+= +=.:%::.*: :=  -=:=.:@=- .=.=::=#*==+:.+.::- :+- +-=#=::+*-= =* --.=:::+-*  --:--:..     .:-::::   
 // ........................:-  -...:..:--.:.=. .-.-==.:..::::..- :.:.....-.--..=  =-:*  =.::..: .:*--::===-=.=:+: #..=-: #-=.-=.. =-....: --*. + -= ...---.# :..+..:.: +:.-==-*..::.-::+. -::.=:=-:-+ -=:..=::*-.: .:-..--::-.:.=-=.:.:-:-.=-:..:=--..=-:=.:+.=: .-::. .-::::::. .:::. % 
 // ...............:..:::..  .  :::.:.:. ---   *: :+..-:-..---: ..==.. := .:.:-.=-. .:.--+ -=-:=+: -  -.--.-  *.-=: .:::.:: : +.=.*  %+* :.#---.- % +:-:.**. *-: - :+-:% =#- : :== :% =:-  ==. =.+:#.- =-*=:#. .+#-.-+.:=.- .#+--#.*:=*:-.-=+.++**. =#*=.=*=---@  --:.:-:.    .::...::  @ 
 // .:.............    ...-=++=. -:..:+ :+:.+-:....-: .:.#  -:--::...---:.=.:::-=+.--. =-.:--= :.-.=.* .-=...::.:  ::+-+ +:+:-:..:--.+...-:: +:==+  ==: += - -:++.== - :==:=---:+:=--=.=.+==:-:=-+..--::+:+..-*+-.= +:-+-==+==.- --=..*::=::.:=:..=:-:.:-=+=-:*  :=-.:-::=*@%.:.:...:. @+ 
 // ............:::-+++-....  .. .::..:+: +=:.:::.+..=-.=. .=..:...=::-:.:-.-: .-.:: - =- -...:-==: .= +  -*. +-:-=:::. .-=.-..** -:*  -+:==:.. ..* = =+:+:.- + += %..:=  ::*: -:. ...--.+  ==* ..:-..--. .-.-= ..+ -== = .+..=-.-:=-::-+::=+.-.+::::-=-:=*::+-  -:::--:::=@=-.:::... =:. 
 // ...:------:.         ..:....: -::..:-  : ::.=-.:..:--..: :+:.--.: :-:..-:---::.-:: ..:- :.:. =. .:- -+-..=..#-+  +:-:.+..=:  -= - -:: := =# =  -.:.+.. + :=- . =*::+===- .:*..-# =: : -=:.= --=.=:.-+* :-.+=+.-+=.#:++=:-+.-+=.*=.-=::=::+:-.**-:=-:==:-:#  --::.-=-::.  .-=+:.. :+-: 
 // ...........:  .@@@@@   ..:.... ::::.:-:-.= .. ....=.:-:.:-:--=.--.=.-*:..:-. =-.+:--:=:.-:....=-= ==.=:=.+=: .:=.: -+.::---. +=-#*=-.::--..+=.:*+%+-.***-=-##@=. .- :=.:+=..:* . =-:-=:.:--:=: .==#=:.--=.::-..::. :+..-:.:-.==:-=+:::-:*..:-+-:..==-==:*  :-:::.:=*#*#%@@@=. ...#-:- 
 // .:......:. =@@=@  @@@@.......:..::..-:-: :-=+=---.-. ::..  .-. :=:-.::..--=.-::-..=:. ::...:::.:.. = ::.- .: :: .:.: ::.: -:--  - -=#*==+*-::=%: . %=   += -::#:.--*. :: +:+=--# -=:::*:.: ::+-= : :=-...::=-.=..-=:.==.:---.-: -. :=:==.+:=.=.:+=.+-.-=: .=:::::.  :=++-   .:::*---= 
 // .....:....   @  ..     .::....: .:.:.=.:=.--. ...--=:...+==:..:: :-.:= -:...- ..: ===:-:::=.-:--:+:.:...:.-+..=+==+=--=::+: : + -%= - %. # +: +  = .= +..= @: *:--. ==-.*:: ... :=.--.::+#--:=: *= = :.-=-==.=-.:-+= .:=..-:= -:--.=-+::.=+.-=..--=:--==  -::::::::.     .:::.:=.-=-: 
 // .:..........       .......::..:. .::.:= .-.:.==--.. ::--. ::--:..: ::.=..-:..:-:.: .- ::..-:.=. .:=.-= .:::=::.. :  ..:.. :=+ +:-.. =  - + -= =  .   =:..:.--=: :--@: #.= -=:-=: -:=-.- = .+=...+: :=#+..= .=:-+ -=.*.-==-...+=.:=:... ==.:-=-:-.=*=-=*  :-:::::::::::::::::.-+. :-.: 
 // ...............................:.-:::..::::..-:.--:.:..:.-.-::--.=::.:-.+= +.:.--=  :.=--= -:.=-:+:.+::-:+ :-- :=.+::..==.+.=.+.:= -=-:+.--: -:+#+*+:+:.@ - -:+ +-: : . =*... =++:::::::.:==-.-:.:-:- --+:=-- =:.--.=..:+..=:=-=-= =--.=-:-.=---+:--:*      .:::::::::::-::::+:  -=.: 
 // ....:...............     .:.....:. ::..:-::-...: :+. -.-.-.:-...:-:::::=...:.-:-:::=-.. ::.:..-+  :::: *:..: -:.-.--=:+.: +.-: :.==-=:::.::= ++. ..- :.=* ..= .: -- +:--:. .+=   :==*.:-- :=-*-.-===:-+.::.=.:- =: -:-.=+:.:=-:::+.-.==++.=----:::+:*. :*@@@-     ...:::::..=:   :=:: 
 // ..:......:.......... @@%-........:. ::....--.:::.=: ::-.-::.:.-.-..-: :: -:--+..:.:=.== .-.::-.-=.::...::.=-+-.:-  -.:.-: :: -=.:-: =.= .-.-..::=.---= :-.*=:===+.-=.-..:-=:-+-.=-: ::+-.::...-.-:.. -:.=:.:::==:.:=::- :.+-.-==:-=.# ..=-*-.-:=-==+= .:     .=@@@*+-.:    ..    :=.. 
 // ...............:..::  %@ .........:. ::..:...--.=:.-..--.:...:-= - --:: --.:: .=: - .-.=:= :: .. :=.-+-=-.=: -..:=..:-+=.*  = .+. :-  =::*..=== :--  * :=..: . .:--.--++#-:...:.-.:-*..=:+ +: ==.:*+:::.. += =:=-+=.+-=*:--.:::..:=-.=++-..-=-:++:=-  ::::  -#       .-@@@==#*+:  --= 
 // .::.::..........::::.    :.........:. :::.::.:.:..:-:::::=:-:.-..-=:..-- -:.:.:-:=-=..-.-:--:-::+:=: ...:-= -::+.-=:.: -  =--- -+--+--.+ -..:..:= :+---- -.=.+-  =-.:-.  .-=:=:: :-:-..--:.=-+ =:: .:--+:=+:-.::-=.:--= -=:--:.:-.:-:-.-.:=.=:-----  =-:-::@@@*@@@@@@-     +==-===-:- 
 // ::..........:....::::.:..:.....:...... ::.:..::..:: .:.. :. ..-:.:.:--::=...:=..     : :-..-.#: .. :=-.+- .-  : = .- +.=++ ..:#:.  .+. :-  = :+:= -=.+=:+:=-- .+.:.#  ==@.=* . *=:: +.+: -*  :=- *#.:.-.- .:.*-...--:. ::= :-++::.:.=-:::-:+-.::=*  --:-::..+-@@: @+ ...: +===-.::::- 
 // ........................................::.::.::::::::.-.-.-:.:::.:.:...-::-.: =::==.==:::=--. .-:.- :=:.:-.:*-.. =.=..-. -:-.=:.-=:.--:=.:*+.=..#. -: +::- -==::=- :#-- .:--*=:.:-:=.--:::+-=:=:..--.=:=---=.-.==+-=-=.----::+::=-+=.-=-::.==:==  --:::::.       . .....#+====-:.:*= 
 // ......................................:  ::..:- ::....:-.....:: :: ...:- - :-=- := :: -:.:+. . =-:.:..+ ::-+. .=.+::=..- #. +. := + = :+-=: .=.:. :* =.+..- : +: ::.:. . -: .... -...:= =. :- :- =:*::-.-= .. +....= - :-.:. +. - .:=:.::=---:=+  --:::::..::.... ..-=-:*=----==-:..- 
 // ..................................... ....::..::-::--.-..::-.=:.=.=-::::.:-:.-. ::..--.::.:::--=..-::-. =::..-=..-.:-.-.--.:=-::-.:.:.:. + -..+.-.-+:= =::-.=..-.--=--+=:.=.-+-.+=:=:-+.-=--.:--:..+-.:-:-=:+=--:.--:--==::---:--+=:==-:=:::-+=  ::::::::.:::::::+=@--.-+====-==-.::. 
 // ..................................  .-:::. ::::.:...:...+.--..:...:...: :--= :=+:=:-:: .+---...-.= .::-+ :....... ==.:.:  =+-: ---++:.+=-==-=+- =-. :+-: .--.=--.-: =  --:::* :#  : + .-=..+-=.-+=-. ::+.:+ .:..-=.*.=: :+:.:::--.--- :+-.:=+: .-:::.:..    ..::::.--. .-=----===-*@# 
 // .:.........................   .---::.....:. .::.:::-.--.-...:- :..:.-.::::  :-....::: :=. .-.:-.:-:-.:::.==:-. ::-:-..=.=:::.:+ - .::: : :- -- ...--::+.--:+: -:--=-:---.--.:- =---:+:.=-.-.-.:-:.--..=:-:-:-:=---.-.-:---=.=:-:--:=:-:--==== .:::..::::*@@@-::.....  ..:=====--=:..: 
 // .:....................  ...=+-.      .....:...:.::.::.:..:.....-=:..::...:.:- :: . .:- : ==.:: ::= :--. = =.=--:--- -:+ .+ -.-:..#-+ :.+: =:=-.::+:.:..-=-: .:*=: :=*:=:-- --++--.. =:.-=::-=- ..:..=+..:::::-:.=*-.-*+=-..=..:=::.::---::-+  -::::::::.     .-=@..+  ...-==----=---= 
 // .:..................:-=--:.     @@@@   ...... .:::.::.:..:::::.:..-:.:..:-::.-.-#.=:.-::-.::- =.:.::=.=.- =.-..:--::=.::::=-..:=. .:==.-.=.+ - == ::-:----.-.- .:-.-:::.:--=:.:.:=---:-:-:=.= :--:=:-::.:=--=.=:-- ---:.:-:-:-:.-:--=:::-:+  -:::..:::::.-:.... ..=#-  ..:==--====++* 
 // .:....................  ....# @@- @#@@.......:..::.:::.-::..:.:.:-..::=..:.:-.-..::...::.:::-:= ::=. .-..:. #..=. : -.-+:.::--:=:.:-:- . : +:-....:..--: .:=:..-.==..: =+:--.-:=:: -:= -.=..---:+.= - ::::...-.::==:..::-:-:-::+-.-=..:::-+  -:::-:...:.:::----:---=*. ...:+==---+##. 
 //  ..::.................:.... .*@#.*.    .......:. .::::.:.::.::-.:.:::...-...:.:-:.:.--:=.::.:..:-::.-..:=.- -.=-.+ ::= :-=: :-::.+:-:=.-+:--:-=-:.-:-:.-:-:-.--:::::.-:::.-..:-..--:+-.-:-.=::..:-:-----:::.-.=-:: -++=.-=::::-.-=::-:-:==*   -:::::-::-::-:..   .:-*+  ...-+=+*#+. . 
 // .......................:....       .........:..:. .:.::.-:.:. .-..:..=:.::-.-....:.:.: ...-=.:.:.-.-:.= .=.:..: :.::.:-.: .:.::...: .: -: .-.: =-::=:::.*::.- = --=-:-:.-=..+--:+ . :..:::::::=-.:..-:=::=*:.---:-+.=.-:=-:.=:=.:=.::--===+%=  .:-:::::=-:.. +%#=  .-#-    .=**. -    
 // .  ......................:...  ........   .......:  .:..-.::-.:.-.:..:.:.:...::..-.=::-::-..:.--:::::-.:.- -:-:-.+:.-::-:.- -::--.... =--:=-.-: =.-.-:.:: =.---:.- .- :-::-:=..:.:=-:::-::::-..::-=.-:.--: ::-:.:.::.:::.:::--:=---==:-=--=+%@-  .--::..:: .=-=%*+==.=== :+.  =   === 
 // :*:. .....................:..#.......   +=........:. .::..::.:..:..-.-..:-.--.:-.:::.-: :.=.:: -::.::-:+.::: : =..:. - -.:--.:-:.------.:.::.-+ =.:.:*=:..-:..--:--===-.::: :-:.:::::. :--.-:=--:.::.-..::=+-::--= ::=--==:-..:-.:-:::::=====+#@=   :-::. -+=:.   **--=+. -:   **+: . 
 // ..-..........................  .......@%: ........... ...::..::.:..-...-.:......:::.:-.::.::.::-.-:.:: ..- .--:-..::=- +.::-:::-:-:.-.:-::.-:.= +..=...::-:-.:-.:. =..=.--.:-:::::=::-=+:-:-:..::::=-:-::...-.::::+.::::-.:-:-:-=-::-.--==*+==++#%@    . *#::-=-##%#+. %+*=%@@=  .=:: 
 // += :+.  .....................::..:...    ............. -:..--.....:..-..-.:::.:....:.-.-..::-..:.-:.::--::-.:.:.::: -- +.:.:.:. =.-:.::::: *.- -....---::-.....-:+=::::..:-:::::=.:::::::.= :=.--:::::-----:.:-:-.=:=-=--:-:-.-.-::.:.:. -=+=*==-=*%%+ = +=.=-@:::=-==*:-=   .:=:::.. 
 // .%:* .+.  ....................::.:......................:....-.:::.:..:.:.::...:..-.:..::.::.:.:.-: :.:.- .:.::+.:..=:.= --.-.=..:.=.:.:=..+ :-..=.=:.=.:.:=-=..- ::::::=:-.:.:::--=::.-: -:::=.-.=.::-:.---:..-:::.: =:.-.:--.-.:::.--.:.::-:+=++==+@.  +@* ..:=.-...:. . . .:.=..-. 
 //  .-@. --::. ..............:.....:... ................. =-....:.....::.:.::.::.:::.-:..:.::=:-.:::-::+..:.-:-::.:::.:.:..:-..=.....:-..=::..+:-:=+..:::-.=..:::-:-:::-:-:::.::::..:.:-:-::-:--:::..:==-:-==.::-:-:.=:.-::.-::.:.-:--:::::.+. =++-+-+=*%  -#@%.:=--+-===*+%%=%-=% :.:.: 
 // ..   +@ -=:   ....................: :+....::........  -=.:...::-:..-:.::::::::..:.:.:.:.:..:.-.-.-....-::..: -.-:::.-:.+.:::- ===:-.:-..-.:..:.:-:.=.=.-.::..::..:-:.:.:.::.::-::--.:..:.:-::::.-+--.::...:::- -:-:=-:.-.:.--:-...:-:-.-:.-::  :*#*+#+ ::*+@:%:%=%*%@#==***%#+**+#=%* 
 // .#.+. %.*  *+:   ...............   .   ...........  .*-:-::::..:::...:..::.::.:::.:..::.:..:..:..:: -.::--.:-...:.-:..-..::-:-:..:..:..:::.:.::..::.-.-:::-.-:-.:-::::=:.:-:--:::-.:=:.-:::..:::..:.:::-::--:-::....:-:---:-..:.-:::::::::::-.+..-=*%: =:%#+:=.+=#*-#+=-+*+##+%*+#-=- 
 //   ..= - %@.= .*=.               .@*--.  ......... .++=..-::..:.:.--..:.:::::.::.:..:.::.-:.:.::.:..:-:.-..:.=.::-..: :..:=..:.::-.:-:::.:.--::.:.::--:.:::::-.::.:.-:.:.-::..::.:.:....:::::-.:.::.--::-.:.::..:.=:::..:=..:::::-..--:=:-.:-:.:=.:.=   +*@@#-%=@+=*.#-.*.:- :   . : . 
 // ::  . .   -@=.= =%+*=-::... -##=.  :#+-:  ....   -+..-:-.:.:.:-::.:-.:.:..:::.:::::::..-...:.:.::..:.:...:..:.:.:.::::.:. ::::.:-.::..-:.-:::.=.:::..:.:::-.::-..::.::-::::-:::-.::::-::::::-:=::::::.::::::::--::.-::-::::-::::::.-.::.--.-::--==  :.+@@* ....:==#+##%#:.--:-.=:.+.* 
 // .* =: : .    +@ :+   = :=*+-..:@#%+. ...=:   ..=*-::::-....:.:-:-:.-.:.:.......:::.::.....::.::.::.::.--:::::.:-=:::.-:::-.:.:.:.-.....-::..-....-::::.::...:::::-:-:::::::::.::.:..::.::::::::::::....         ...:::-.::.:-.::.=-:-:::-.=-==*. - ..   ++==-*-@+#%+**.%- #:: +- :-.. 
 // -.: -.+.-: ..   :@**.+*. --+.    +=.-..  %#..:-:.:.-::.:..-::::.::::.-..:.-.::..:.:::::::.:..:.:.:.:::.-.::::.:.:...:.:::..-.:::.-.-::..:.:::-:=::.::.-..:::.-.::::-.::..:.::::::::.:::--:::..:......::-+==+*+-:..   ..:.:::::::::-:.---=--:  . .:..=..* .*:++:=..=:*=-%=.:#*+@@#@@-* 
 // .@-.-: . # +...:   ::=::-.==+@.=.= :-:+::+% ...==:::.: ..-..:-:-.:..::.:::::::-:......:.:.-.....:.::.:...::.:.-.:::::::::.:::::::.::.::.::.:...:.::::.:::::.::::.:.:.. ..         ...+:  ..:: ......:--::-....:---==.  .::--.--:::-:=-::   : .::-= #- -.      .---=*%%#@# -#--%*  =   
 //  **.+:=:....:.+.-. .- - .. :*-#+=::-..:...+%= .:----..:::.::.-...-.-.:::-::..::.::.::.:.....::::::::::::::..:....:..:.:.:.:...:..::::..:::::::.:::....:..            :=-..*%*+-#@#*=--..:-.::.....:-------::::::..:-==:   ...:::---:.  .:.--:-. = .-...  +  #.#@+@*:*+ %: #.. - -.+:. 
 // @+%-*=.#.--::.:- = -. =-:@%#@=.*--+.--.: . :@: ..:. ..:::-:-:=.:.:.:.:.:.::.:::-::-:::::::::...   ..::::.::::.:::::::::.:.::::::::::::-::.-.:::...:-::---=**%@%##%%***#%*==-::.. :+:   .:--=-==-..::-:----::.:-:-+--:::-==. :::.   .:::.:....%:.:....:= *# @. +   +.%#*%*--. =+:@ %=  
 // :@*+@#+%*+=.#---.+--*:#%=@@-.::.-==.-:--.+..*%  ...::::.-:..:-::-::::-.-:::::::::.::-:::..  :+.-@..:.........:.....:.....:............:...:....:::::::::.         :=.     .:::..   ....  ..=+++#+::::::::-:::.::.....:.:--=.  ...:.::.....-.:: ::.:- =: * .- ..  : +*@#@%.#=:% -- -   
 // .:.-@ #*=%#%##+@:@%@@##@* =: *  :      .-::  =-     :-:::--...-:::-..:::-::.::-::--:....... .=.-@ .::::::::..:::::::....:::::::::::::..:::::.:::::::::...:-.::--:... .:--....::::.:::.:.....==.....:::::::.:---::--::::::.-:.....-.:..:- #..:...: *..% .:    :- %=.@ + =- =  .  . ::  
 // +==-***# *=:%*++-++.==::-  = +::@.#=-+.===+: -@@@%      -:--=-:.::-.:.-.:-::::::......::::. .-..@  ..:....::::..  ..:......::.........:....:::::...:...:=---.......:+:.....:.::==::::::.........:..-==--::....::::::::::::-: ....:.::.=: +..:. =. +.:- .: -- #:.# :  :    :=.#:+@ *%  
 // ::-=:+*@%%%.+=:*=--+.=  = -+ :      .:::::. :   =%@@@*-      .-:--::-:::.:.--::.......:..... :. @: ...::..:.... =+ ...==.:.::...::.:::::.:.......:.:.:::.....::::..--:::::::....::.::.::........:::::=::::.:::::--=::::-::=:..:...... =..:..-.:@ =: .  -= @-.= ..    -::@ @+.+ ..     
 // -::::.--=#%@@#-*--=:=-- ==%- @+ @ :::=--.   :.       =@@@@%:       .....::-:. :-.:=:..........: *- ...::.:....:+*%=-+.+*:.::....--.:....:::.::::.:::....:::::..:..........:-::..+:::::::.........:.:.:-::::::::::::::::-::=...:.....: =..:..- -- :. :: #..= .. .- #=:@ =- :  . .- =*- 
 //  =@=--:..=. *=+%++*-*=#@@@@+ :  =.:#.% -:.+  .. .. -      +#@@@%@*=...::=- ...:...:::::...::. . =- -*-...:.:::..    .. ......:*=.:...:.......:.:::..:.:.:.::::::.::::...:::.:-:::..:...........:::::.---=-::..:::::::::::-....::.::...- .: == :. .. @.::... := %= @ .. ...:-.@:-% .:- 
 // .   =%-=+.---.-+-+#+=% == #+ %+ =       :..+ :: :...: .:     :  ::-*+=+:  -=.:::......::......:.=*  .:...:.:.::.....::.......:. ....:..:..:.:::.......:::::::.:..:..::==::.::..:::::::::::............:----::::::::::::--... :...:..:.: .- +  :..#.== .. -- @- *    .= #*.@..= ..==-+ 
 // -.-.  --==-#-*##. * :*.:% :+ .% %+.@ :...  .. +: =..:. =  =.....:. . .=.  :. .....:::.....::..:. #=  ..::...::::--.:..:::..::...::...:::....:::::..:::::.....:::.:.:....:::::..:..:.:::.::.:............:::-::.-:::::--. ::.#=:.:: -... *: ...* +- .  -- @..:    :% %# =      :+:--:- 
 // ..:.:- .:=.=* #**%@+.@ .#..@ .     . .#.:= :. .= :: :: :# ::..=..: :::= . *. .::....:.:=-:::.... -%: .::...::........:..............::......::...::::.   ......:::::::::::::::::......:.::.::::..........:--:-::.:=-:.. :.--  :....= .:.+  :.=- .  -:.@.::  . =#.%+ :  . .*:=++-::= : 
 //  .....:=: =*#:--  *-:@:.#- #: @ :* .. .. =: *. :. #  -  +..# .-..= :-:....*: .:..::....:....:.::. =%. .........:.....:.::...::.....::..:....:...::.   ..:::.:-.:...:::.::::::.::::::::.::::::..:........:.::---::..  :.-:. :.:....:..=:.. -= :. -.:% ..  . #% #. .  :. %-+-:-.----.   
 // .. ...:...:  #=@@+@: @+ ** =- :  #:.* .. .: =- :. -+ *: .: *- ::.+ :-.:.  ++   .:.:::.:..::::::..  :%+ .:..:::..:..:::.:...:...::::...:::::...::.   .:--:.::::.:.-::...:::::..:-:.::::.:.:..:.:::....:...:.:--..=:: : .-::.#.  :-:.:-+..-:%. : .% -. .-.#*.*    .=..@.--:-..:-:-  -#% 
 //  ... ....:. .   * *% *= -% =% =:  . :  #  .  - .%  : .@..: :+ ::.=:.-.::-:%+=+  ...::....:........:  *%:  ..-:..:::......:..::...:............    .=+-:......::::.:::-:::....:..::.::::::.::::::........ . ..    =@ :*: =..  =::.=+.:. #-..  * *. .:.@=.-  . .@.:+ ----.:-:=- .:#+=== 
 // #=..:....:.:=. .   *..@..+  # .@ -# ---*+=%+-:: += -  -- +  +:.: -+ .=:.:..=-:+  .........:..:......  .#%=   ..........:.................      =%%*:....::.:.:::........::.:.:::::..:.:::::::::.:.    ..+.  .+=:+:+: = -  ::* .:-=..:.*- .: @. .. %:.. .: -%.=: --==---.-:...+%:  :-- 
 //   *+-.. .  .::-==.  . =: @= +  :. #=+*: ...+:+*  = -*  . == ...*. --.--.:: -@.--:      ...:...:.:..:..   =##=    .....::....:......       .+@@@%:   .....:.:...::.:..-:::::...::::::::::::...  . :+.:-.. +@#++- ..-*   : *.:  +.-=-#- . .@..   @.=-  :.+# -  .--+:-+  =:.           . 
 // ..  :==:.:.....:.+=:* .. .. ## +  -==*=-#-::::%. :  #.:-  #.-. .%: .-=--=:-..=@*=.+*:==.  . .........::.    -#%#=:.                  +@@@@@%:    ...:.:::....:::::......:.:.::..:.::..     -*:+-.  +#-#%-.    ..:..+: +.-. . ==+**=...*:=. .#:#.  ..*+ +  -.=+:.= --:-   +@@%*+*#%%%* 
 // .:... .::.. .  .-. .:..#..-  . =@ -   * -+-=-:-+ +-  . #=   .%= .=+::==:--::-. -+-@* : :--..--.     . ...::.    :=+**++++++++#%@@@@@@%=       ......:......:.:.::::::::::::..  .    :*.-+.  .#-:%@.+-    .:.:.::. ..#  .:.++#%#@+..-:*:. :=@. ::.@::=  .-=+:::=::::   .@@#*+*##*-::-: 
 // .........:-:....  .:. :.  %..-    .@..= .=-+#+=:  @ =. .+=+-  *+.  +:.=---:-=::- ..-   @-=#. .- *%+ --                  .......         ...::...:::::.....::...:.... .     : =*.=+:   *+.#@:=.:   .  .:.:-.-:.:.:::=+  @=%#@@*.  .=-.. -#... -+.+.  =--*:--:-:.= . .#%#*+==+++++****+ 
 // :..........::::.... ..:...: .*-.*     *+ +  =- %.    @#-  :#%:  @@*-.*%-:-:--.:=..=.- -=  -+-=@: .*. -:=#+:+#.:=-.::.                :.     .                    -..++.#*:-::  =# :@*--.    . :..:.-:-.:::.:.::.:-+*@* =%@*-.  -:--. *:#: -=.#.  .:%=:+:.=::.   .+@%*==+++=++=++++==+ 
 // ........:.....:..... : .:.:...  .@ :=  : =@ .   #.%+   -@#  .%@+:.+@@ =+==::.:=:::-  -*...+ .:. =-.-+## :+* -+  : :- .+##@+=%@-+%:-*+.#=.-#..=* **.=%-=@-+@@*@#  ::. -+ :@=-@::::.   : ...::.::.-. : ....: :--*#*##*#@: .-  . @.  -:@: .:.@.   .%--+ :.-:.    +@%#*====--==+==+++++++ 
 // ::......:::::.::...::.: .. - .-... .+: -  =.*@ -=  ##@* --@@- ++@@= *@#-.==---:::- =  +  .:.+::::: .  . . :..=:-#===@-.%- += .*  +. -. -  .. .:  .   -  #:  *  @+=-@..- ..  . ..::::::..:.::...: : ..--+-++#*#*#+-+==-%   -@.:-:=@.  ..@ .  .++:-=-.:-    .*@@%*++==++++=-=+++====+*+ 
 // ................::.......:. -.:...: :  %+ .  :+:@#@@ +%%@* :%@+- =@@*-=+*=*::-::..  +=: := :.. +..=:::= =::- -. . . :     :: --.=#-=+*=**:*%:-#@-+#--== :.. .  :    ..:..:-:::.:....::.::.:.-:..:+.#**#*#%#*++-:.:    *@   ::=@.:  -% :  +=-*: ...     .+@%*++==++===+==+=+=++++==-== 
 // ........:::::......:::::...... ...:.... .. @ ..  = @@#%  %@%+ .%@=--*+++##=##**===+=. . :-:..::: :.. ...:.:.-:-::-..-...:: :..: .  .    . .     . .... -.:..--.-.:-:::-::.:..::-::..:--..:=-=+*######**+--...  :: ==@@-%+ .* -  .: -  ++-+ .:.+.    -*@%#+====--=====-=+-====-++*+==: 
 // ...::.......:::..:.:....:::. ...-..-...:..  . +-    @--@%-*:@@+ ++#-.+=+-++**+%@*@%@:.@#+====-..+:.:.:.- -..: : :.-::::::.:-...::=.-::.:: -:-::::-.-:.::...-.:..:.-:::::::--:-.:.:-----+##%**+*#++*-:....  -.++:+=%=-@* @    ::.@..:==: -.:.    .-@@#++=+=======-=+++*=+++====+===*+= 
 // .......:..:..:..:..::::.:..:::.. . .. ...:+-.. : :-  . +#:@@ ++@+- *#*::=-+.*:-=** @ @@@#***%+*+-====:.: ::..:::::.:...: .:.::......:.-.-.-::.:::.:... -..:::-.--::..:.:::-:-===+*+####*#**+**+-:. .:.::#=@@:*++%= ..   @@ ==+*=::=. =... . .-*%%*====-====+====-=-==-==--=+===++=+++ 
 // ..:...::..::........:...:::.:.::::. .::: .  :--- ..*     * +@#@%+@@.-:%**-=::=.+  @#.. ==*#*+*####*####***+-*-:=.--..-....   ::.:... .  .   . . .. - --::::-::.=+--=**+*#####%*%@#***+=+-:.:.   :- @#=%@**--*.:: ..  .-=.@+ -  ::.= *  ...=#@*+++======+===+====+=-===+=++===++===++* 
 // ........:...::..::::...::......:.::::....::. . ..-::::-: :  :  .%-*@#@**:*.-=%:. %@  : # =:+++=*+*########%%%%*#%*%***+*#**#++++=*+=+=*+-+=*=++--++=+++**#**##%########***+*+*==-.::. .  . .* +#@@+@@ = : - ..  ...::-=:-..: :- .. ..::++=+*-+=-===+======++====+-++=+-===+++==-:---- 
 // ...:-:..:..::..:::.::::::.:.:::...::.::::....-:.     :.- .-: +     - *@*-=@:*..=%@  *@-*.= -.:..-- ---+=**++**#%#+%##%%%#%%#@#@#%*%%###%%*%%%@#@%@@%%%@##%##%##=*=++==-=.:..   . -..+.%=@@%#@@@==*.::.:...: .. =--+-::.. ::+  ..:.::========.===+==-==+======+-++===+=+-===--=**=:--= 
 // :......:.:.:..::::::.:.:..:::..:..:.::::::.::...:--:... : .:..::.:  -   =.=--%*=@*. #=:*:#=*=++-:+-:= = :=.. -.-:+--:=-=-+====+++*+=****++*++*+*+====-+--.::... ..:.    . -.=#%%*@@:@+@%===+. .  : . .:.:::.-:::.  ......   ==-==-=-:--:-:--==+=--+=+=-==+====-+==+==++====+===-=--=- 
 // :..:......::...:....:..:..:::...:::..:....:::::....:::::.:.    ...--: .- : =. . .:*  -::#:##.*=.+%++:-@ -.#:%....= - .: . .. ::. = . .   .  . :        . . :  .==+.-+@@@@*@@@*#@.+-=.:..   .. :..:.-=:-.::.:......::::.  .=**++-=---::--:-=----=====*-++=-=+==++-==++-=*=++===-+++==- 
 // :........::..::.:::::..:.::.......:::...::::....::......::::.:. .. . .-.:. .-.#: .     ::*.%+-=#=*.%.*--*.@:*-##*@-#%#@:+@.@:# %+-.%=+.@=#*+#-@==@=@@@*@%@@@@@@@%@@@@@@+##-- : ..+ .. .  .:.. :.: ...  ..:-..:::-==-..:*****+=----:::.-----==--=--++==+=+==+++=+=-=+==++-+======:=+== 
 // ..::.::::.::.:..::.:..:.:.:.::::......:::.......:..:.:.......::-.:...  .:.-.:...:- -:+..     =.- #.+.*=%-#.+:==+.+#.:-@-% *=#@=+=@:*@@@@*@+@@.%@ @%*-+=@+#+:=:....         . . . .:...:::..:=:....::.::.:.::-==-:   :=*#=++===-====-:.:-::-----=-++=+==+=++=-+=+======++=+=*-+==++==+ 
 // ....:...:.....::.:.....::...:....:..........:..:::....:...........::::.. .     .:.. : .... ..       .. ..-:-:--+-.-:*:*.+.::.-:#:- * ...... . .. - - :..     .. : ...:.....:....:...:........:.:..::::---=-:.   ..:...-==-======+--=-:-::::--=+=====-+=+-==--==-=+-=+=-=-++++==++-==+ 
 //                                                                                    
