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
/*
*    J2534.cs - C# PassThru Implementation - based on the original by Jason Martin
*
*
*
*
*/
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace J2534
{
	// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public enum ConfigParameter
	{
		DATA_RATE = 0x01,
		// CAN FD data-phase bitrate (J2534-2 v05.00). DATA_RATE above sets the arbitration
		// phase; this sets the faster data phase (e.g. 2_000_000 = 2 Mbit/s). Confirm the
		// exact config ID against the DLL's .ini / GET_CONFIG — vendors differ. Placeholder
		// value in the J2534-2 extended range until verified against the target VCI.
		FD_CAN_DATA_PHASE_RATE = 0x00008100,
		LOOPBACK = 0x03,
		NODE_ADDRESS = 0x04,
		NETWORK_LINE = 0x05,
		P1_MIN = 0x06,
		P1_MAX = 0x07,
		P2_MIN = 0x08,
		P2_MAX = 0x09,
		P3_MIN = 0x0A,
		P3_MAX = 0x0B,
		P4_MIN = 0x0C,
		P4_MAX = 0x0D,
		W1 = 0x0E,
		W2 = 0x0F,
		W3 = 0x10,
		W4 = 0x11,
		W5 = 0x12,
		TIDLE = 0x13,
		TINIL = 0x14,
		TWUP = 0x15,
		PARITY = 0x16,
		BIT_SAMPLE_POINT = 0x17,
		SYNC_JUMP_WIDTH = 0x18,
		W0_MIN = 0x19,
		T1_MAX = 0x1A,
		T2_MAX = 0x1B,
		T4_MAX = 0x1C,
		T5_MAX = 0x1D,
		ISO15765_BS = 0x1E,
		ISO15765_STMIN = 0x1F,
		DATA_BITS = 0x20,
		FIVE_BAUD_MOD = 0x21,
		BS_TX = 0x22,
		STMIN_TX = 0x23,
		T3_MAX = 0x24,
		ISO15765_WFT_MAX = 0x25,
		W1_MIN = 0X26,
		W2_MIN = 0X27,
		W3_MIN = 0X28,
		W4_MAX = 0X29,
		N_BR_MIN = 0X2A,
		ISO15765_PAD_VALUE = 0X2B,
		N_AS_MAX = 0X2C,
		N_AR_MAX = 0X2D,
		N_BS_MAX = 0X2E,
		N_CR_MAX = 0X2F,
		N_CS_MIN = 0X30,
		ECHO_PHYSCIAL_CHANNEL_TX = 0X31,
		CAN_MIXED_FORMAT = 0x00008000,
		J1962_PINS = 0x00008001,
		SW_CAN_HS_DATA_RATE = 0x00008010,
		SW_CAN_SPEEDCHANGE_ENABLE = 0x00008011,
		SW_CAN_RES_SWITCH = 0x00008012,
		ACTIVE_CHANNELS = 0x00008020,
		SAMPLE_RATE = 0x00008021,
		SAMPLES_PER_READING = 0x00008022,
		READINGS_PER_MSG = 0x00008023,
		AVERAGING_METHOD = 0x00008024,
		SAMPLE_RESOLUTION = 0x00008025,
		INPUT_RANGE_LOW = 0x00008026,
		INPUT_RANGE_HIGH = 0x00008027,
		UEB_T0_MIN = 0x00008028,
		UEB_T1_MAX = 0x00008029,
		UEB_T2_MAX = 0x0000802A,
		UEB_T3_MAX = 0x0000802B,
		UEB_T4_MIN = 0x0000802C,
		UEB_T5_MAX = 0x0000802D,
		UEB_T6_MAX = 0x0000802E,
		UEB_T7_MIN = 0x0000802F,
		UEB_T7_MAX = 0x00008030,
		UEB_T9_MIN = 0x00008031,
		J1939_PINS = 0x0000803D,
		J1708_PINS = 0x0000803E,
		J1939_T1 = 0x0000803F,
		J1939_T2 = 0x00008040,
		J1939_T3 = 0x00008041,
		J1939_T4 = 0x00008042,
		J1939_BRDCST_MIN_DELAY = 0x00008043,
		TP2_0_T_BR_INT = 0x00008044,
		TP2_0_T_E = 0x00008045,
		TP2_0_MNTC = 0x00008046,
		TP2_0_T_CTA = 0x00008047,
		TP2_0_MNCT = 0x00008048,
		TP2_0_MNTB = 0x00008049,
		TP2_0_MNT = 0x0000804A,
		TP2_0_T_Wait = 0x0000804B,
		TP2_0_T1 = 0x0000804C,
		TP2_0_T3 = 0x0000804D,
		TP2_0_IDENTIFER = 0x0000804E,
		TP2_0_RXIDPASSIVE = 0x0000804F,
		OBDX_DelayBetweenCANFrames = 0x00010000, //in MICROSECONDS. Default is 500uS
		OBDX_FlowControlDelayMessage = 0x00010001, //in micros. Default is 1mS as of v1.03
	}

	public interface IJ2534
	{
		bool LoadLibrary(J2534Device device);
		bool FreeLibrary();
		// Device-readiness probe. On the interface so a relay (RemotePassThru) can answer it for
		// the far-end device exactly like the local J2534Functions does.
		bool IsDLLLoaded { get; }

		J2534Err PassThruOpen(IntPtr name, ref uint deviceId);
		J2534Err PassThruClose(uint deviceId);
		J2534Err PassThruConnect(uint deviceId, ProtocolID protocolId, ConnectFlag flags, BaudRate baudRate, ref uint channelId);
		J2534Err PassThruDisconnect(int channelId);
		J2534Err PassThruReadMsgs(int channelId, IntPtr msgs, ref int numMsgs, int timeout);
		J2534Err PassThruStartPeriodicMsg(int channelId, IntPtr msg, ref int msgId, int timeInterval);
		J2534Err PassThruStopPeriodicMsg(int channelId, int msgId);
		J2534Err PassThruStartMsgFilter(int channelid, FilterType filterType, IntPtr maskMsg, IntPtr patternMsg, IntPtr flowControlMsg, ref int filterId);
		J2534Err PassThruStopMsgFilter(int channelId, int filterId);
		J2534Err PassThruSetProgrammingVoltage(int deviceId, PinNumber pinNumber, uint voltage);
		J2534Err PassThruReadVersion(int deviceId, IntPtr firmwareVersion, IntPtr dllVersion, IntPtr apiVersion);
		J2534Err PassThruGetLastError(IntPtr errorDescription);
		J2534Err PassThruIoctl(int channelId, int ioctlID, IntPtr input, IntPtr output);

		J2534Err PassThruQueueMsgs(int channelId, IntPtr msg, ref int numMsgs);
		J2534Err PassThruScanForDevices(ref int DeviceCount);
		J2534Err PassThruGetNextDevice(ref SDEVICE Device);
		J2534Err PassThruLogicalConnect(int PhyscialChannelId, ProtocolID protocolId, ConnectFlag flags, ref IntPtr ChannelDescriptor, ref int LogicalChannelId);
		J2534Err PassThruLogicalDisconnect(int LogicalChannelId);
		J2534Err PassThruSelect(ref IntPtr ChannelSetPtr, uint SelectType, uint Timeout);
		J2534Err PassThruWriteMsgs(int channelId, IntPtr msg, ref int numMsgs, int timeout);




	}

	public interface IJ2534Extended : IJ2534
	{
		J2534Err GetConfig(int channelId, ref List<SConfig> config);
		J2534Err SetConfig(int channelId, ref List<SConfig> config);

		J2534Err SW_CAN_BusSpeed(int ChannelID, int Option);
		J2534Err ReadBatteryVoltage(int deviceId, ref int voltage);

		J2534Err ReadProgrammingVoltage(int deviceId, ref int voltage);

		J2534Err FiveBaudInit(int channelId, byte targetAddress, ref byte keyword1, ref byte keyword2);
		J2534Err FastInit(int channelId, PassThruMsg txMsg, ref PassThruMsg rxMsg);
		J2534Err ClearTxBuffer(int channelId);
		J2534Err ClearRxBuffer(int channelId);
		J2534Err ClearPeriodicMsgs(int channelId);
		J2534Err ClearMsgFilters(int channelId);
		J2534Err ClearFunctMsgLookupTable(int channelId);
		J2534Err AddToFunctMsgLookupTable(int channelId);
		J2534Err DeleteFromFunctMsgLookupTable(int channelId);
		J2534Err ReadAllMessages(int channelId, int numMsgs, int timeout, out List<PassThruMsg> messages, bool readUntilTimeout);
	}

	public enum Ioctl
	{
		GET_CONFIG = 0x01,
		SET_CONFIG = 0x02,
		READ_VBATT = 0x03,
		FIVE_BAUD_INIT = 0x04,
		FAST_INIT = 0x05,
		CLEAR_TX_BUFFER = 0x07,
		CLEAR_RX_BUFFER = 0x08,
		CLEAR_PERIODIC_MSGS = 0x09,
		CLEAR_MSG_FILTERS = 0x0A,
		CLEAR_FUNCT_MSG_LOOKUP_TABLE = 0x0B,
		ADD_TO_FUNCT_MSG_LOOKUP_TABLE = 0x0C,
		DELETE_FROM_FUNCT_MSG_LOOKUP_TABLE = 0x0D,
		READ_PROG_VOLTAGE = 0x0E,
		BUS_ON = 0x0F,
		SW_CAN_HS = 0x8000, //Request swcan Highspeed
		SW_CAN_NS = 0x8001, //Request swcan low speed
		SET_POLL_RESPONSE = 0x8002, //aldl
		BECOME_MASTER = 0x8003, //aldl
		START_REPEAT_MESSAGE = 0x8004,
		QUERY_REPEAT_MESSAGE = 0x8005,
		STOP_REPEAT_MESSAGE = 0x8006,
		GET_ConnectedDevice_CONFIG = 0x8007,
		SET_ConnectedDevice_CONFIG = 0x8008,
		PROTECT_J1939_ADDR = 0x8009,
		REQUEST_CONNECTION = 0x800A,
		TEARDOWN_CONNECTION = 0x800B,
		GET_Device_INFO = 0x800C,
		GET_PROTOCOL_INFO = 0x800D,
		OBDX_AvailableDevices = 0x10000001,
	}

	public enum GET_DEVICE_INFO_Defines : uint
	{
		OBDX_UniqueSerial_Part1 = 0x10000001,
		OBDX_UniqueSerial_Part2 = 0x10000002,
		OBDX_UniqueSerial_Part3 = 0x10000003,
		OBDX_UniqueSerial_Part4 = 0x10000004,
		SERIAL_NUMBER = 0x00000001,
		J1850PWM_SUPPORTED = 0x00000002,
		J1850VPW_SUPPORTED = 0x00000003,
		ISO9141_SUPPORTED = 0x00000004,
		ISO14230_SUPPORTED = 0x00000005,
		CAN_SUPPORTED = 0x00000006,
		ISO15765_SUPPORTED = 0x00000007,
		SCI_A_ENGINE_SUPPORTED = 0x00000008,
		SCI_A_TRANS_SUPPORTED = 0x00000009,
		SCI_B_ENGINE_SUPPORTED = 0x0000000A,
		SCI_B_TRANS_SUPPORTED = 0x0000000B,
		SW_ISO15765_SUPPORTED = 0x0000000C,
		SW_CAN_SUPPORTED = 0x0000000D,
		GM_UART_SUPPORTED = 0x0000000E,
		UART_ECHO_BYTE_SUPPORTED = 0x0000000F,
		HONDA_DIAGH_SUPPORTED = 0x00000010,
		J1939_SUPPORTED = 0x00000011,
		J1708_SUPPORTED = 0x00000012,
		TP2_0_SUPPORTED = 0x00000013,
		J2610_SUPPORTED = 0x00000014,
		ANALOG_IN_SUPPORTED = 0x00000015,
		MAX_NON_VOLATILE_STORAGE = 0x00000016,
		SHORT_TO_GND_J1962 = 0x00000017,
		PGM_VOLTAGE_J1962 = 0x00000018,
		J1850PWM_PS_J1962 = 0x00000019,
		J1850VPW_PS_J1962 = 0x0000001A,
		ISO9141_PS_K_LINE_J1962 = 0x0000001B,
		ISO9141_PS_L_LINE_J1962 = 0x0000001C,
		ISO14230_PS_K_LINE_J1962 = 0x0000001D,
		ISO14230_PS_L_LINE_J1962 = 0x0000001E,
		CAN_PS_J1962 = 0x0000001F,
		ISO15765_PS_J1962 = 0x00000020,
		SW_CAN_PS_J1962 = 0x00000021,
		SW_ISO15765_PS_J1962 = 0x00000022,
		GM_UART_PS_J1962 = 0x00000023,
		UART_ECHO_BYTE_PS_J1962 = 0x00000024,
		HONDA_DIAGH_PS_J1962 = 0x00000025,
		J1939_PS_J1962 = 0x00000026,
		J1708_PS_J1962 = 0x00000027,
		TP2_0_PS_J1962 = 0x00000028,
		J2610_PS_J1962 = 0x00000029,
		J1939_PS_J1939 = 0x0000002A,
		J1708_PS_J1939 = 0x0000002B,
		ISO9141_PS_K_LINE_J1939 = 0x0000002C,
		ISO9141_PS_L_LINE_J1939 = 0x0000002D,
		ISO14230_PS_K_LINE_J1939 = 0x0000002E,
		ISO14230_PS_L_LINE_J1939 = 0x0000002F,
		J1708_PS_J1708 = 0x00000030,
		FT_CAN_SUPPORTED = 0x00000031,
		FT_ISO15765_SUPPORTED = 0x00000032,
		FT_CAN_PS_J1962 = 0x00000033,
		FT_ISO15765_PS_J1962 = 0x00000034,
		J1850PWM_SIMULTANEOUS = 0x00000035,
		J1850VPW_SIMULTANEOUS = 0x00000036,
		ISO9141_SIMULTANEOUS = 0x00000037,
		ISO14230_SIMULTANEOUS = 0x00000038,
		CAN_SIMULTANEOUS = 0x00000039,
		ISO15765_SIMULTANEOUS = 0x0000003A,
		SCI_A_ENGINE_SIMULTANEOUS = 0x0000003B,
		SCI_A_TRANS_SIMULTANEOUS = 0x0000003C,
		SCI_B_ENGINE_SIMULTANEOUS = 0x0000003D,
		SCI_B_TRANS_SIMULTANEOUS = 0x0000003E,
		SW_ISO15765_SIMULTANEOUS = 0x0000003F,
		SW_CAN_SIMULTANEOUS = 0x00000040,
		GM_UART_SIMULTANEOUS = 0x00000041,
		UART_ECHO_BYTE_SIMULTANEOUS = 0x00000042,
		HONDA_DIAGH_SIMULTANEOUS = 0x00000043,
		J1939_SIMULTANEOUS = 0x00000044,
		J1708_SIMULTANEOUS = 0x00000045,
		TP2_0_SIMULTANEOUS = 0x00000046,
		J2610_SIMULTANEOUS = 0x00000047,
		ANALOG_IN_SIMULTANEOUS = 0x00000048,
		PART_NUMBER = 0x00000049,
		FT_CAN_SIMULTANEOUS = 0x0000004A,
		FT_ISO15765_SIMULTANEOUS = 0x0000004B,
	}



	[Flags]
	public enum RxStatus
	{
		NONE = 0x00000000,
		TX_MSG_TYPE = 0x00000001,
		START_OF_MESSAGE = 0x00000002,
		RX_BREAK = 0x00000004,
		TX_INDICATION_SUCCESS = 0x00000008,
		ISO15765_PADDING_ERROR = 0x00000010,
		ERROR_INDICATION = 0x20,
		BUFFER_OVERFLOW = 0x40,
		ISO15765_ADDR_TYPE = 0x00000080,
		CAN_29BIT_ID = 0x00000100,
		TX_FAILED = 0x200,
		SW_CAN_HV_TX = 0x00000400,
		SW_CAN_NS_RX = 0x00040000,
		SW_CAN_HS_RX = 0x00020000,
		SW_CAN_HV_RX = 0x00010000,
		// ---- CAN FD (J2534-2 v05.00) — received-frame status (confirm per DLL .ini) ----
		// Tells the caller what each received frame actually was: FD framing, whether the
		// data phase used bit-rate switch, and the error-state indicator bit.
		CAN_FD_FORMAT = 0x00100000,
		CAN_FD_BRS = 0x00200000,
		CAN_FD_ESI = 0x00400000

	}

	[Flags]
	public enum ConnectFlag
	{
		NONE = 0x0000,
		ISO9141_K_LINE_ONLY = 0x1000,
		CAN_ID_BOTH = 0x0800,
		ISO9141_NO_CHECKSUM = 0x0200,
		CAN_29BIT_ID = 0x0100,
		FULL_DUPLEX = 0x1
	}

	[Flags]
	public enum TxFlag : uint
	{
		NONE = 0x00000000,
		SCI_TX_VOLTAGE = 0x00800000,
		SCI_MODE = 0x00400000,
			// 0x200 is context-sensitive per J2534 v0404: ISO14230 = WAIT_P3_MIN_ONLY, ISO9141 = NO_CHECKSUM
		WAIT_P3_MIN_ONLY = 0x00000200,
			ISO9141_NO_CHECKSUM = 0x00000200,
		CAN_29BIT_ID = 0x00000100,
		ISO15765_ADDR_TYPE = 0x00000080,
		ISO15765_FRAME_PAD = 0x00000040,
		SW_CAN_HV_TX = 0x00000400,
		// ---- CAN FD (J2534-2 v05.00) — per-message framing flags ----
		// Confirm exact bit values against the loaded DLL's .ini; v05.00 numbering varies
		// per vendor. CAN_FD_FORMAT = transmit as CAN FD; CAN_FD_BRS = enable the faster
		// data-phase bit-rate switch on this frame.
		CAN_FD_FORMAT = 0x00040000,
		CAN_FD_BRS = 0x00080000
	}

	public enum ProtocolID : uint
	{
		J1850VPW = 0x01,
		J1850PWM = 0x02,
		ISO9141 = 0x03,
		ISO14230 = 0x04,
		CAN = 0x05,
		ISO15765 = 0x06,
		SCI_A_ENGINE = 0x07,
		SCI_A_TRANS = 0x08,
		SCI_B_ENGINE = 0x09,
		SCI_B_TRANS = 0x0A,
		ISO15765_LOGICAL = 0x200,
		//J2534-2 protocols
		J1850VPW_PS = 0x8000,
		J1850PWM_PS = 0x8001,
		ISO9141_PS = 0x8002,
		ISO14230_PS = 0x8003,
		CAN_PS = 0x8004,
		ISO15765_PS = 0x8005,
		J2610_PS = 0x8006,
		SW_ISO15765_PS = 0x8007,
		SW_CAN_PS = 0x8008,
		GM_UART_PS = 0x8009,
		UART_ECHO_BYTE_PS = 0x800A,
		HONDA_DIAGH_PS = 0x800B,
		J1939_PS = 0x800C,
		J1708_PS = 0x800D,
		TP2_0_PS = 0x800E,
		FT_CAN_PS = 0x800F,
		FT_ISO15765_PS = 0x8010,
		// ---- J2534-2 v05.00 DoIP / ISO 13400 IP transport (Route A hooks) ----
		// Used only when a v05.00 PassThru DLL exposes DoIP itself; the native Route-B
		// client in J2534\DoIp does not need these. Confirm exact values against the
		// vendor DLL headers/.ini — v05.00 numbering varies per implementation.
		IPV6 = 0x8015,
		IPV4 = 0x8016,
		UDP = 0x8017,
		TCP_CLIENT = 0x8018
	}

	public enum BaudRate
	{
		ISO9141_4800 = 4800,    // SSM2 K-Line (Subaru pre-2008)
		ISO9141_9600 = 9600,    // K-Line general
		ISO9141_10400 = 10400,
		ISO9141_10000 = 10000,


		ISO14230_10400 = 10400,
		ISO14230_10000 = 10000,


		J1850PWM_41600 = 41600,
		J1850PWM_83300 = 83300,

		J1850VPW_10400 = 10400,
		J1850VPW_41600 = 41600,

		CAN_125000 = 125000,
		CAN_250000 = 250000,
		CAN_500000 = 500000,
		CAN_33333 = 33333,
		CAN_83333 = 83333,


		ISO15765_125000 = 125000,
		ISO15765_250000 = 250000,
		ISO15765_500000 = 500000,

		GMUART_8192 = 8192,
		GMUART_160 = 160,

	}

	public enum PinNumber : uint
	{
		AUX = 0,
		PIN_1 = 1,
		PIN_3 = 3,
		PIN_6 = 6,
		PIN_9 = 9,
		PIN_11 = 11,
		PIN_12 = 12,
		PIN_13 = 13,
		PIN_14 = 14,
		PIN_15 = 15
	}

	public enum PinVoltage : uint
	{
		FEPS_VOLTAGE = 18000,
		SHORT_TO_GROUND = 0xFFFFFFFE,
		VOLTAGE_OFF = 0xFFFFFFFF
	}

	public enum FilterType
	{
		PASS_FILTER = 0x01,
		BLOCK_FILTER = 0x02,
		FLOW_CONTROL_FILTER = 0x03
	}

	public enum J2534Err : uint
	{
		STATUS_NOERROR = 0x00,
		ERR_NOT_SUPPORTED = 0x01,
		ERR_INVALID_CHANNEL_ID = 0x02,
		ERR_INVALID_PROTOCOL_ID = 0x03,
		ERR_NULL_PARAMETER = 0x04,
		ERR_INVALID_FLAGS = 0x06,
		ERR_FAILED = 0x07,
		ERR_ConnectedDevice_NOT_CONNECTED = 0x08,
		ERR_TIMEOUT = 0x09,
		ERR_INVALID_MSG = 0x0A,
		ERR_INVALID_TIME_INTERVAL = 0x0B,
		ERR_EXCEEDED_LIMIT = 0x0C,
		ERR_INVALID_MSG_ID = 0x0D,
		ERR_ConnectedDevice_IN_USE = 0x0E,
		ERR_INVALID_IOCTL_ID = 0x0F,
		ERR_BUFFER_EMPTY = 0x10,
		ERR_BUFFER_FULL = 0x11,
		ERR_BUFFER_OVERFLOW = 0x12,
		ERR_PIN_INVALID = 0x13,
		ERR_CHANNEL_IN_USE = 0x14,
		ERR_MSG_PROTOCOL_ID = 0x15,
		ERR_INVALID_FILTER_ID = 0x16,
		ERR_NO_FLOW_CONTROL = 0x17,
		ERR_NOT_UNIQUE = 0x18,
		ERR_INVALID_BAUDRATE = 0x19,
		ERR_INVALID_ConnectedDevice_ID = 0x1A,
		ERR_ConnectedDevice_NOT_OPEN = 0X1B,
		ERR_NULL_REQUIRED = 0X1C,
		ERR_FILTER_TYPE_NOT_SUPPORTED = 0X1D,
		ERR_IOCTL_PARAM_ID_NOT_SUPPORTED = 0X1E,
		ERR_VOLTAGE_IN_USE = 0X1F,
		ERR_PIN_IN_USE = 0X20,
		ERR_INIT_FAILED = 0X21,
		ERR_OPEN_FAILED = 0X22,
		ERR_BUFFER_TOO_SMALL = 0X23,
		ERR_LOG_CHAN_NOT_ALLOWED = 0X24,
		ERR_SELECT_TYPE_NOT_SUPPORTED = 0X25,
		ERR_CONCURRENT_API_CALL = 0X26,
		ERR_OEM_VOLTAGE_TOO_HIGH = 0x77,
		ERR_OEM_VOLTAGE_TOO_LOW = 0x78,
		ERR_ACCESS_VIOLATION = 0x1000,
		ERR_DLL_NOT_LOADED = 0x1001,
		ERR_RESOURCE_IN_USE = 0x1002,
		ERR_OUT_OF_MEMORY = 0x1999,
		ERR_ALDL_HEARTBEAT_NOT_FOUND = 0x5000,
		ERR_NO_RESPONSE_FROM_MODULE = 0x5001
	}

	public class J2534Device
	{
		public string Vendor { get; set; }
		public string Name { get; set; }
		public string FunctionLibrary { get; set; }
		public string ConfigApplication { get; set; }
		public int CAN { get; set; }
		public int ISO14230 { get; set; }
		public int ISO15765 { get; set; }
		public int ISO9141 { get; set; }
		public int J1850PWM { get; set; }
		public int J1850VPW { get; set; }
		public int SCI_A_ENGINE { get; set; }
		public int SCI_A_TRANS { get; set; }
		public int SCI_B_ENGINE { get; set; }
		public int SCI_B_TRANS { get; set; }
		public int CAN_PS { get; set; }
		public int FT_CAN_PS { get; set; }
		public int FT_ISO15765_PS { get; set; }
		public int GM_UART_PS { get; set; }
		public int ISO14230_PS { get; set; }
		public int ISO15765_PS { get; set; }
		public int ISO9141_PS { get; set; }
		public int SW_CAN_PS { get; set; }
		public int SW_ISO15765_PS { get; set; }
		public int J1850VPW_PS { get; set; }

		//public bool IsCANSupported
		//{
		//    get { return CAN > 0; }
		//}

		//public bool IsISO14230Supported
		//{
		//    get { return ISO14230 > 0; }
		//}

		//public bool IsISO15765Supported
		//{
		//    get { return ISO15765 > 0; }
		//}

		//public bool IsISO9141Supported
		//{
		//    get { return ISO9141 > 0; }
		//}

		//public bool IsJ1850PWMSupported
		//{
		//    get { return J1850PWM > 0; }
		//}

		//public bool IsJ1850VPWSupported
		//{
		//    get { return J1850VPW > 0; }
		//}

		//public bool IsSCI_A_ENGINESupported
		//{
		//    get { return SCI_A_ENGINE > 0; }
		//}

		//public bool IsSCI_A_TRANSSupported
		//{
		//    get { return SCI_A_TRANS > 0; }
		//}

		//public bool IsSCI_B_ENGINESupported
		//{
		//    get { return SCI_B_ENGINE > 0; }
		//}

		//public bool IsSCI_B_TRANSSupported
		//{
		//    get { return SCI_B_TRANS > 0; }
		//}

		// ---- ELM327 serial adapter support ----
		/// <summary>True if this device is an ELM327 serial adapter (not a J2534 PassThru device).</summary>
		public bool IsELM327 { get; set; }
		/// <summary>COM port name (e.g. "COM3") when IsELM327 is true.</summary>
		public string ComPort { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}

	internal static class NativeMethods
	{
		[DllImport("kernel32.dll")]
		public static extern IntPtr LoadLibrary(string dllToLoad);

		[DllImport("kernel32.dll")]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

		[DllImport("kernel32.dll")]
		public static extern bool FreeLibrary(IntPtr hModule);
	}

	public class J2534DllWrapper
	{
		private IntPtr m_pDll;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate int PassThruOpen(IntPtr name, ref uint deviceId);
		public PassThruOpen Open;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate int PassThruClose(uint deviceId);
		public PassThruClose Close;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate int PassThruScanForDevices(ref int DeviceCount);
		public PassThruScanForDevices ScanForDevices;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate int PassThruGetNextDevice(ref SDEVICE Device);
		public PassThruGetNextDevice GetNextDevice;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate int PassThruConnect(uint deviceId, ProtocolID protocolId, ConnectFlag flags, BaudRate baudRate, ref uint channelId);
		public PassThruConnect Connect;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate int PassThruDisconnect(int channelId);
		public PassThruDisconnect Disconnect;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate int PassThruReadMsgs(int channelId, IntPtr pMessages, ref int numMsgs, int timeout);
		public PassThruReadMsgs ReadMsgs;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate int PassThruWriteMsgs(int channelId, IntPtr pMessages, ref int numMsgs, int timeout);
		public PassThruWriteMsgs WriteMsgs;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate int PassThruQueueMsgs(int channelId, IntPtr pMessages, ref int numMsgs);
		public PassThruQueueMsgs WriteQueueMsgs;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate int PassThruLogicalConnect(int PhyscialChannelId, ProtocolID protocolId, ConnectFlag flags, ref IntPtr ChannelDescriptor, ref int LogicalChannelId);
		public PassThruLogicalConnect ConnectLogical;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate int PassThruLogicalDisconnect(int LogicalChannelId);
		public PassThruLogicalDisconnect DisconnectLogical;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate int PassThruSelect(ref IntPtr ChannelSetPtr, uint SelectType, uint Timeout);
		public PassThruSelect SelectChannelToCheck;



		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate int PassThruStartPeriodicMsg(
			int channelId, IntPtr msg, ref int msgId, int timeInterval);
		public PassThruStartPeriodicMsg StartPeriodicMsg;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate int PassThruStopPeriodicMsg(int channelId, int msgId);
		public PassThruStopPeriodicMsg StopPeriodicMsg;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate int PassThruStartMsgFilter
			(
			int channelid,
			int filterType,
			IntPtr maskMsg,
			IntPtr patternMsg,
			IntPtr flowControlMsg,
			ref int filterId
			);
		public PassThruStartMsgFilter StartMsgFilter;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate int PassThruStartPassBlockMsgFilter
			(
			int channelid,
			int filterType,
			ref PassThruMsg uMaskMsg,
			ref PassThruMsg uPatternMsg,
			int nada,
			ref int filterId
			);
		public PassThruStartPassBlockMsgFilter StartPassBlockMsgFilter;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate int PassThruStopMsgFilter(int channelId, int filterId);
		public PassThruStopMsgFilter StopMsgFilter;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate int PassThruSetProgrammingVoltage(int deviceId, uint pinNumber, uint voltage);
		public PassThruSetProgrammingVoltage SetProgrammingVoltage;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate int PassThruReadVersion(
			int deviceId, IntPtr firmwareVersion, IntPtr dllVersion, IntPtr apiVersion);
		public PassThruReadVersion ReadVersion;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate int PassThruGetLastError(IntPtr errorDescription);
		public PassThruGetLastError GetLastError;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate int PassThruIoctl(int channelId, int ioctlID, IntPtr input, IntPtr output);
		public PassThruIoctl Ioctl;




		public bool LoadJ2534Library(string path)
		{
			//this DLL does not play nice and causes a loaderlock
			//TODO fix this properly
			//string p = Path.GetFileName(path);
			//if (p.Contains("BVTX4J32.dll"))
			//{
			//    // return false;
			//}
			m_pDll = NativeMethods.LoadLibrary(path);

			if (m_pDll == IntPtr.Zero)
				return false;

			IntPtr pAddressOfFunctionToCall = NativeMethods.GetProcAddress(m_pDll, "PassThruOpen");
			if (pAddressOfFunctionToCall != IntPtr.Zero)
				Open = (PassThruOpen)Marshal.GetDelegateForFunctionPointer(
					pAddressOfFunctionToCall,
					typeof(PassThruOpen));
			else
				return false;

			//  return true;

			pAddressOfFunctionToCall = NativeMethods.GetProcAddress(m_pDll, "PassThruClose");
			if (pAddressOfFunctionToCall != IntPtr.Zero)
				Close = (PassThruClose)Marshal.GetDelegateForFunctionPointer(
					pAddressOfFunctionToCall,
					typeof(PassThruClose));
			else
				return false;

			pAddressOfFunctionToCall = NativeMethods.GetProcAddress(m_pDll, "PassThruConnect");
			if (pAddressOfFunctionToCall != IntPtr.Zero)
				Connect = (PassThruConnect)Marshal.GetDelegateForFunctionPointer(
					pAddressOfFunctionToCall,
					typeof(PassThruConnect));
			else
				return false;

			pAddressOfFunctionToCall = NativeMethods.GetProcAddress(m_pDll, "PassThruDisconnect");
			if (pAddressOfFunctionToCall != IntPtr.Zero)
				Disconnect = (PassThruDisconnect)Marshal.GetDelegateForFunctionPointer(
					pAddressOfFunctionToCall,
					typeof(PassThruDisconnect));
			else
				return false;


			pAddressOfFunctionToCall = NativeMethods.GetProcAddress(m_pDll, "PassThruReadMsgs");
			if (pAddressOfFunctionToCall != IntPtr.Zero)
				ReadMsgs = (PassThruReadMsgs)Marshal.GetDelegateForFunctionPointer(
					pAddressOfFunctionToCall,
					typeof(PassThruReadMsgs));
			else
				return false;

			pAddressOfFunctionToCall = NativeMethods.GetProcAddress(m_pDll, "PassThruWriteMsgs");
			if (pAddressOfFunctionToCall != IntPtr.Zero)
				WriteMsgs = (PassThruWriteMsgs)Marshal.GetDelegateForFunctionPointer(
					pAddressOfFunctionToCall,
					typeof(PassThruWriteMsgs));
			else
				return false;

			pAddressOfFunctionToCall = NativeMethods.GetProcAddress(m_pDll, "PassThruStartPeriodicMsg");
			if (pAddressOfFunctionToCall != IntPtr.Zero)
				StartPeriodicMsg = (PassThruStartPeriodicMsg)Marshal.GetDelegateForFunctionPointer(
					pAddressOfFunctionToCall,
					typeof(PassThruStartPeriodicMsg));
			else
				return false;

			pAddressOfFunctionToCall = NativeMethods.GetProcAddress(m_pDll, "PassThruStopPeriodicMsg");
			if (pAddressOfFunctionToCall != IntPtr.Zero)
				StopPeriodicMsg = (PassThruStopPeriodicMsg)Marshal.GetDelegateForFunctionPointer(
					pAddressOfFunctionToCall,
					typeof(PassThruStopPeriodicMsg));
			else
				return false;

			pAddressOfFunctionToCall = NativeMethods.GetProcAddress(m_pDll, "PassThruStartMsgFilter");
			if (pAddressOfFunctionToCall != IntPtr.Zero)
				StartMsgFilter = (PassThruStartMsgFilter)Marshal.GetDelegateForFunctionPointer(
					pAddressOfFunctionToCall,
					typeof(PassThruStartMsgFilter));


			pAddressOfFunctionToCall = NativeMethods.GetProcAddress(m_pDll, "PassThruStartMsgFilter");
			if (pAddressOfFunctionToCall != IntPtr.Zero)
				StartPassBlockMsgFilter = (PassThruStartPassBlockMsgFilter)Marshal.GetDelegateForFunctionPointer(
					pAddressOfFunctionToCall,
					typeof(PassThruStartPassBlockMsgFilter));
			else
				return false;

			pAddressOfFunctionToCall = NativeMethods.GetProcAddress(m_pDll, "PassThruStopMsgFilter");
			if (pAddressOfFunctionToCall != IntPtr.Zero)
				StopMsgFilter = (PassThruStopMsgFilter)Marshal.GetDelegateForFunctionPointer(
					pAddressOfFunctionToCall,
					typeof(PassThruStopMsgFilter));
			else
				return false;

			pAddressOfFunctionToCall = NativeMethods.GetProcAddress(m_pDll, "PassThruSetProgrammingVoltage");
			if (pAddressOfFunctionToCall != IntPtr.Zero)
				SetProgrammingVoltage = (PassThruSetProgrammingVoltage)Marshal.GetDelegateForFunctionPointer(
					pAddressOfFunctionToCall,
					typeof(PassThruSetProgrammingVoltage));
			else
				return false;

			pAddressOfFunctionToCall = NativeMethods.GetProcAddress(m_pDll, "PassThruReadVersion");
			if (pAddressOfFunctionToCall != IntPtr.Zero)
				ReadVersion = (PassThruReadVersion)Marshal.GetDelegateForFunctionPointer(
					pAddressOfFunctionToCall,
					typeof(PassThruReadVersion));
			else
				return false;

			pAddressOfFunctionToCall = NativeMethods.GetProcAddress(m_pDll, "PassThruGetLastError");
			if (pAddressOfFunctionToCall != IntPtr.Zero)
				GetLastError = (PassThruGetLastError)Marshal.GetDelegateForFunctionPointer(
					pAddressOfFunctionToCall,
					typeof(PassThruGetLastError));
			else
				return false;

			pAddressOfFunctionToCall = NativeMethods.GetProcAddress(m_pDll, "PassThruIoctl");
			if (pAddressOfFunctionToCall != IntPtr.Zero)
				Ioctl = (PassThruIoctl)Marshal.GetDelegateForFunctionPointer(
					pAddressOfFunctionToCall,
					typeof(PassThruIoctl));
			else
				return false;


			//Below are for J2534 v5, not suitable for 4.04 supported above
			//pAddressOfFunctionToCall = NativeMethods.GetProcAddress(m_pDll, "PassThruScanForDevices");
			//if (pAddressOfFunctionToCall != IntPtr.Zero)
			//    ScanForDevices = (PassThruScanForDevices)Marshal.GetDelegateForFunctionPointer(
			//        pAddressOfFunctionToCall,
			//        typeof(PassThruScanForDevices));
			//else
			//    return false;


			//pAddressOfFunctionToCall = NativeMethods.GetProcAddress(m_pDll, "PassThruGetNextDevice");
			//if (pAddressOfFunctionToCall != IntPtr.Zero)
			//    GetNextDevice = (PassThruGetNextDevice)Marshal.GetDelegateForFunctionPointer(
			//         pAddressOfFunctionToCall,
			//         typeof(PassThruGetNextDevice));
			//else
			//    return false;

			//pAddressOfFunctionToCall = NativeMethods.GetProcAddress(m_pDll, "PassThruQueueMsgs");
			//if (pAddressOfFunctionToCall != IntPtr.Zero)
			//    WriteQueueMsgs = (PassThruQueueMsgs)Marshal.GetDelegateForFunctionPointer(
			//         pAddressOfFunctionToCall,
			//         typeof(PassThruQueueMsgs));
			//else
			//    return false;

			//pAddressOfFunctionToCall = NativeMethods.GetProcAddress(m_pDll, "PassThruLogicalConnect");
			//if (pAddressOfFunctionToCall != IntPtr.Zero)
			//    ConnectLogical = (PassThruLogicalConnect)Marshal.GetDelegateForFunctionPointer(
			//         pAddressOfFunctionToCall,
			//         typeof(PassThruLogicalConnect));
			//else
			//    return false;

			//pAddressOfFunctionToCall = NativeMethods.GetProcAddress(m_pDll, "PassThruLogicalDisconnect");
			//if (pAddressOfFunctionToCall != IntPtr.Zero)
			//    DisconnectLogical = (PassThruLogicalDisconnect)Marshal.GetDelegateForFunctionPointer(
			//         pAddressOfFunctionToCall,
			//         typeof(PassThruLogicalDisconnect));
			//else
			//    return false;

			//pAddressOfFunctionToCall = NativeMethods.GetProcAddress(m_pDll, "PassThruSelect");
			//if (pAddressOfFunctionToCall != IntPtr.Zero)
			//    SelectChannelToCheck = (PassThruSelect)Marshal.GetDelegateForFunctionPointer(
			//         pAddressOfFunctionToCall,
			//         typeof(PassThruSelect));
			//else
			//    return false;




			return true;
		}

		public bool FreeLibrary()
		{
			if (m_pDll != IntPtr.Zero)
			{
				NativeMethods.FreeLibrary(m_pDll);
				m_pDll = IntPtr.Zero;
			}
			return true;
		}
	}

	public class J2534Functions : IJ2534
	{
		public override string ToString()
		{
			return "Copyright 2026 (c) Tester Present Specialist Automotive Solutions - J2534 API DLL - Tester Engineering Suite .";
		}


		protected J2534Device _ConnectedDevice;
		protected J2534DllWrapper _J2534DllWrapper;
		bool _IsDLLLoaded = false;
		public bool IsDLLLoaded
		{
			get
			{
				return _IsDLLLoaded;
			}
		}

		public string DeviceName
		{
			get
			{
				return _ConnectedDevice.Name;
			}
		}

		public bool LoadLibrary(J2534Device device)
		{
			try
			{
				_ConnectedDevice = device;
				_J2534DllWrapper = new J2534DllWrapper();
				_IsDLLLoaded = _J2534DllWrapper.LoadJ2534Library(_ConnectedDevice.FunctionLibrary);
				return _IsDLLLoaded;
			}
			catch (Exception)
			{
				_IsDLLLoaded = false;
				return _IsDLLLoaded;
			}

		}

		public bool FreeLibrary()
		{
			_IsDLLLoaded = false;
			if (_J2534DllWrapper != null) return _J2534DllWrapper.FreeLibrary();
			return true;

		}

		public J2534Err PassThruOpen(IntPtr name, ref uint deviceId)
		{
			try
			{
				//access violation exception (When scantool has locked up)
				if (!IsDLLLoaded) return J2534Err.ERR_DLL_NOT_LOADED;
				var result = (J2534Err)_J2534DllWrapper.Open(name, ref deviceId);

				return result;
			}
			catch (Exception ex)
			{
				return J2534Err.ERR_ACCESS_VIOLATION;
			}

		}

		public J2534Err PassThruClose(uint deviceId)
		{
			try
			{
				if (!IsDLLLoaded) return J2534Err.ERR_DLL_NOT_LOADED;
				return (J2534Err)_J2534DllWrapper.Close(deviceId);
			}
			catch (Exception)
			{ return J2534Err.ERR_ACCESS_VIOLATION; }

		}


		public J2534Err PassThruScanForDevices(ref int DeviceCount)
		{
			try
			{
				if (!IsDLLLoaded) return J2534Err.ERR_DLL_NOT_LOADED;
				return (J2534Err)_J2534DllWrapper.ScanForDevices(ref DeviceCount);
			}
			catch (Exception)
			{ return J2534Err.ERR_ACCESS_VIOLATION; }
		}

		public J2534Err PassThruGetNextDevice(ref SDEVICE Device)
		{
			try
			{
				if (!IsDLLLoaded) return J2534Err.ERR_DLL_NOT_LOADED;
				return (J2534Err)_J2534DllWrapper.GetNextDevice(ref Device);
			}
			catch (Exception)
			{ return J2534Err.ERR_ACCESS_VIOLATION; }

		}

		public J2534Err PassThruQueueMsgs(int channelId, IntPtr msgs, ref int numMsgs)
		{
			try
			{
				if (!IsDLLLoaded) return J2534Err.ERR_DLL_NOT_LOADED;
				return (J2534Err)_J2534DllWrapper.WriteQueueMsgs(channelId, msgs, ref numMsgs);
			}
			catch (Exception)
			{ return J2534Err.ERR_ACCESS_VIOLATION; }

		}


		public J2534Err PassThruLogicalConnect(int PhyscialChannelId, ProtocolID protocolId, ConnectFlag flags, ref IntPtr ChannelDescriptor, ref int LogicalChannelId)
		{
			try
			{
				if (!IsDLLLoaded) return J2534Err.ERR_DLL_NOT_LOADED;
				return (J2534Err)_J2534DllWrapper.ConnectLogical(PhyscialChannelId, protocolId, flags, ref ChannelDescriptor, ref LogicalChannelId);
			}
			catch (Exception)
			{ return J2534Err.ERR_ACCESS_VIOLATION; }

		}

		public J2534Err PassThruLogicalDisconnect(int LogicalChannelId)
		{
			try
			{
				if (!IsDLLLoaded) return J2534Err.ERR_DLL_NOT_LOADED;
				return (J2534Err)_J2534DllWrapper.DisconnectLogical(LogicalChannelId);
			}
			catch (Exception)
			{ return J2534Err.ERR_ACCESS_VIOLATION; }

		}

		public J2534Err PassThruSelect(ref IntPtr ChannelSetPtr, uint SelectType, uint Timeout)
		{
			try
			{
				if (!IsDLLLoaded) return J2534Err.ERR_DLL_NOT_LOADED;
				return (J2534Err)_J2534DllWrapper.SelectChannelToCheck(ref ChannelSetPtr, SelectType, Timeout);
			}
			catch (Exception)
			{ return J2534Err.ERR_ACCESS_VIOLATION; }
		}


		public J2534Err PassThruConnect(uint deviceId, ProtocolID protocolId, ConnectFlag flags, BaudRate baudRate, ref uint channelId)
		{
			try
			{
				if (!IsDLLLoaded) return J2534Err.ERR_DLL_NOT_LOADED;
				return (J2534Err)_J2534DllWrapper.Connect(deviceId, protocolId, flags, baudRate, ref channelId);
			}
			catch (Exception)
			{ return J2534Err.ERR_ACCESS_VIOLATION; }

		}

		public J2534Err PassThruDisconnect(int channelId)
		{
			try
			{
				if (!IsDLLLoaded) return J2534Err.ERR_DLL_NOT_LOADED;
				return (J2534Err)_J2534DllWrapper.Disconnect(channelId);
			}
			catch (Exception)
			{ return J2534Err.ERR_ACCESS_VIOLATION; }

		}

		public J2534Err PassThruReadMsgs(int channelId, IntPtr msgs, ref int numMsgs, int timeout)
		{
			try
			{
				if (!IsDLLLoaded) return J2534Err.ERR_DLL_NOT_LOADED;
				return (J2534Err)_J2534DllWrapper.ReadMsgs(channelId, msgs, ref numMsgs, timeout);
			}
			catch (Exception)
			{ return J2534Err.ERR_ACCESS_VIOLATION; }
		}

		public J2534Err PassThruWriteMsgs(int channelId, IntPtr msgs, ref int numMsgs, int timeout)
		{
			try
			{
				if (!IsDLLLoaded) return J2534Err.ERR_DLL_NOT_LOADED;
				return (J2534Err)_J2534DllWrapper.WriteMsgs(channelId, msgs, ref numMsgs, timeout);
			}
			catch (Exception)
			{ return J2534Err.ERR_ACCESS_VIOLATION; }

		}

		public J2534Err PassThruStartPeriodicMsg(int channelId, IntPtr msg, ref int msgId, int timeInterval)
		{
			try
			{
				if (!IsDLLLoaded) return J2534Err.ERR_DLL_NOT_LOADED;
				return (J2534Err)_J2534DllWrapper.StartPeriodicMsg(channelId, msg, ref msgId, timeInterval);
			}
			catch (Exception)
			{ return J2534Err.ERR_ACCESS_VIOLATION; }

		}

		public J2534Err PassThruStopPeriodicMsg(int channelId, int msgId)
		{
			try
			{
				if (!IsDLLLoaded) return J2534Err.ERR_DLL_NOT_LOADED;
				return (J2534Err)_J2534DllWrapper.StopPeriodicMsg(channelId, msgId);
			}
			catch (Exception)
			{ return J2534Err.ERR_ACCESS_VIOLATION; }

		}

		public J2534Err PassThruStartMsgFilter(int channelid, FilterType filterType, IntPtr maskMsg, IntPtr patternMsg, IntPtr flowControlMsg, ref int filterId)
		{
			try
			{
				if (!IsDLLLoaded) return J2534Err.ERR_DLL_NOT_LOADED;
				return (J2534Err)_J2534DllWrapper.StartMsgFilter(channelid, (int)filterType, maskMsg, patternMsg, flowControlMsg, ref filterId);
			}
			catch (Exception)
			{ return J2534Err.ERR_ACCESS_VIOLATION; }
		}

		public J2534Err PassThruStopMsgFilter(int channelId, int filterId)
		{
			try
			{
				if (!IsDLLLoaded) return J2534Err.ERR_DLL_NOT_LOADED;
				return (J2534Err)_J2534DllWrapper.StopMsgFilter(channelId, filterId);
			}
			catch (Exception)
			{ return J2534Err.ERR_ACCESS_VIOLATION; }

		}

		public J2534Err PassThruSetProgrammingVoltage(int deviceId, PinNumber pinNumber, uint voltage)
		{
			try
			{
				if (!IsDLLLoaded) return J2534Err.ERR_DLL_NOT_LOADED;
				return (J2534Err)_J2534DllWrapper.SetProgrammingVoltage(deviceId, (uint)pinNumber, voltage);
			}
			catch (Exception)
			{ return J2534Err.ERR_ACCESS_VIOLATION; }
		}

		public J2534Err PassThruReadVersion(int deviceId, IntPtr firmwareVersion, IntPtr dllVersion, IntPtr apiVersion)
		{
			try
			{
				if (!IsDLLLoaded) return J2534Err.ERR_DLL_NOT_LOADED;
				return (J2534Err)_J2534DllWrapper.ReadVersion(deviceId, firmwareVersion, dllVersion, apiVersion);
			}
			catch (Exception)
			{ return J2534Err.ERR_ACCESS_VIOLATION; }

		}

		public J2534Err PassThruGetLastError(IntPtr errorDescription)
		{
			try
			{
				if (!IsDLLLoaded) return J2534Err.ERR_DLL_NOT_LOADED;
				return (J2534Err)_J2534DllWrapper.GetLastError(errorDescription);
			}
			catch (Exception)
			{ return J2534Err.ERR_ACCESS_VIOLATION; }
		}

		public J2534Err PassThruIoctl(int channelId, int ioctlID, IntPtr input, IntPtr output)
		{
			try
			{
				if (!IsDLLLoaded) return J2534Err.ERR_DLL_NOT_LOADED;
				return (J2534Err)_J2534DllWrapper.Ioctl(channelId, ioctlID, input, output);
			}
			catch (Exception)
			{ return J2534Err.ERR_ACCESS_VIOLATION; }
		}
	}

	public class J2534FunctionsExtended : J2534Functions, IJ2534Extended
	{

		public J2534Err GetConfig(int channelId, ref List<SConfig> config)
		{
			IntPtr listPtr = IntPtr.Zero;
			IntPtr input = IntPtr.Zero;
			try
			{
				SConfigList sConfigList = new SConfigList();
				listPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SConfig)) * config.Count);
				sConfigList.ListPtr = listPtr;
				sConfigList.Count = config.Count;
				for (int i = 0; i < config.Count; i++)
				{
					Marshal.StructureToPtr(config[i], new IntPtr(sConfigList.ListPtr.ToInt64() + Marshal.SizeOf(typeof(SConfig)) * i), false);
				}

				IntPtr output = IntPtr.Zero;
				input = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SConfigList)));

				Marshal.StructureToPtr(sConfigList, input, false);

				var err = (J2534Err)_J2534DllWrapper.Ioctl(channelId, (int)Ioctl.GET_CONFIG, input, output);

				var configList = input.AsStruct<SConfigList>().GetList();

				config = new List<SConfig>();
				for (int i = 0; i < configList.Count; i++)
				{
					config.Add(configList[i]);
				}

				return err;
			}
			catch (OutOfMemoryException)
			{ return J2534Err.ERR_OUT_OF_MEMORY; }
			finally
			{
				if (input != IntPtr.Zero) Marshal.FreeHGlobal(input);
				if (listPtr != IntPtr.Zero) Marshal.FreeHGlobal(listPtr);
			}
		}


		public J2534Err SetConfig(int channelId, ref List<SConfig> config)
		{
			IntPtr input = IntPtr.Zero;
			SConfigList sConfigList = new SConfigList { ListPtr = IntPtr.Zero };
			try
			{
				sConfigList.ListPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SConfig)) * config.Count);
				sConfigList.Count = config.Count;
				for (int i = 0; i < config.Count; i++)
				{
					Marshal.StructureToPtr(config[i], new IntPtr(sConfigList.ListPtr.ToInt64() + Marshal.SizeOf(typeof(SConfig)) * i), false);
				}

				IntPtr output = IntPtr.Zero;//output is null, not used
				input = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SConfigList)));
				Marshal.StructureToPtr(sConfigList, input, false);

				return (J2534Err)_J2534DllWrapper.Ioctl(channelId, (int)Ioctl.SET_CONFIG, input, output);
			}
			catch (OutOfMemoryException)
			{ return J2534Err.ERR_OUT_OF_MEMORY; }
			finally
			{
				// Free unconditionally on every path (matches GetConfig) — the old code freed
				// only on the success path, leaking both HGlobals on any exception.
				if (input != IntPtr.Zero) Marshal.FreeHGlobal(input);
				if (sConfigList.ListPtr != IntPtr.Zero) Marshal.FreeHGlobal(sConfigList.ListPtr);
			}
		}

		public J2534Err SW_CAN_BusSpeed(int ChannelId, int Option)
		{
			try
			{
				IntPtr input = IntPtr.Zero;
				IntPtr output = IntPtr.Zero;

				if (Option == 0)
				{
					return (J2534Err)_J2534DllWrapper.Ioctl(ChannelId, (int)Ioctl.SW_CAN_HS, input, output);
				}
				else
				{
					return (J2534Err)_J2534DllWrapper.Ioctl(ChannelId, (int)Ioctl.SW_CAN_NS, input, output);
				}


			}
			catch (OutOfMemoryException)
			{ return J2534Err.ERR_OUT_OF_MEMORY; }

		}

		public J2534Err ReadBatteryVoltage(int deviceId, ref int voltage)
		{
			try
			{
				IntPtr input = IntPtr.Zero;
				IntPtr output = Marshal.AllocHGlobal(8);
				try
				{
					J2534Err returnValue = (J2534Err)_J2534DllWrapper.Ioctl(deviceId, (int)Ioctl.READ_VBATT, input, output);
					if (returnValue == J2534Err.STATUS_NOERROR)
					{
						voltage = Marshal.ReadInt32(output);
					}
					return returnValue;
				}
				finally { Marshal.FreeHGlobal(output); }   // free even if the Ioctl throws
			}
			catch (OutOfMemoryException)
			{ return J2534Err.ERR_OUT_OF_MEMORY; }

		}

		public J2534Err ReadProgrammingVoltage(int deviceId, ref int voltage)
		{
			try
			{
				IntPtr input = IntPtr.Zero;
				IntPtr output = Marshal.AllocHGlobal(8);
				try
				{
					J2534Err returnValue = (J2534Err)_J2534DllWrapper.Ioctl(deviceId, (int)Ioctl.READ_PROG_VOLTAGE, input, output);
					if (returnValue == J2534Err.STATUS_NOERROR)
					{
						voltage = Marshal.ReadInt32(output);
					}
					return returnValue;
				}
				finally { Marshal.FreeHGlobal(output); }   // free even if the Ioctl throws
			}
			catch (OutOfMemoryException)
			{ return J2534Err.ERR_OUT_OF_MEMORY; }

		}

		public J2534Err FiveBaudInit(int channelId, byte targetAddress, ref byte keyword1, ref byte keyword2)
		{
			IntPtr inputDataPtr = IntPtr.Zero;
			IntPtr outputDataPtr = IntPtr.Zero;
			IntPtr input = IntPtr.Zero;
			IntPtr output = IntPtr.Zero;
			try
			{
				// Allocate input SByteArray: 1-byte payload = target address
				inputDataPtr = Marshal.AllocHGlobal(1);
				Marshal.WriteByte(inputDataPtr, targetAddress);
				SByteArray inputArray = new SByteArray { NumOfBytes = 1, BytePtr = inputDataPtr };

				// Allocate output SByteArray: 2-byte result = keyword1 + keyword2
				outputDataPtr = Marshal.AllocHGlobal(2);
				Marshal.WriteByte(outputDataPtr, 0, 0);
				Marshal.WriteByte(outputDataPtr, 1, 0);
				SByteArray outputArray = new SByteArray { NumOfBytes = 2, BytePtr = outputDataPtr };

				// Marshal structures to unmanaged memory
				input = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SByteArray)));
				output = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SByteArray)));
				Marshal.StructureToPtr(inputArray, input, false);
				Marshal.StructureToPtr(outputArray, output, false);

				J2534Err returnValue = (J2534Err)_J2534DllWrapper.Ioctl(channelId, (int)Ioctl.FIVE_BAUD_INIT, input, output);

				if (returnValue == J2534Err.STATUS_NOERROR)
				{
					SByteArray resultArray = (SByteArray)Marshal.PtrToStructure(output, typeof(SByteArray));
					if (resultArray.BytePtr != IntPtr.Zero && resultArray.NumOfBytes >= 2)
					{
						keyword1 = Marshal.ReadByte(resultArray.BytePtr, 0);
						keyword2 = Marshal.ReadByte(resultArray.BytePtr, 1);
					}
				}

				return returnValue;
			}
			catch (OutOfMemoryException)
			{ return J2534Err.ERR_OUT_OF_MEMORY; }
			catch (Exception)
			{ return J2534Err.ERR_ACCESS_VIOLATION; }
			finally
			{
				if (inputDataPtr != IntPtr.Zero) Marshal.FreeHGlobal(inputDataPtr);
				if (outputDataPtr != IntPtr.Zero) Marshal.FreeHGlobal(outputDataPtr);
				if (input != IntPtr.Zero) Marshal.FreeHGlobal(input);
				if (output != IntPtr.Zero) Marshal.FreeHGlobal(output);
			}
		}

		public J2534Err FastInit(int channelId, PassThruMsg txMsg, ref PassThruMsg rxMsg)
		{
			IntPtr input = IntPtr.Zero;
			IntPtr output = IntPtr.Zero;
			try
			{
				input = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(PassThruMsg)));
				output = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(PassThruMsg)));

				Marshal.StructureToPtr(txMsg, input, false);
				PassThruMsg emptyMsg = new PassThruMsg();
				Marshal.StructureToPtr(emptyMsg, output, false);

				J2534Err returnValue = (J2534Err)_J2534DllWrapper.Ioctl(channelId, (int)Ioctl.FAST_INIT, input, output);
				if (returnValue == J2534Err.STATUS_NOERROR)
				{
					rxMsg = (PassThruMsg)Marshal.PtrToStructure(output, typeof(PassThruMsg));
				}

				return returnValue;
			}
			catch (OutOfMemoryException)
			{ return J2534Err.ERR_OUT_OF_MEMORY; }
			catch (Exception)
			{ return J2534Err.ERR_ACCESS_VIOLATION; }
			finally
			{
				if (input != IntPtr.Zero) Marshal.FreeHGlobal(input);
				if (output != IntPtr.Zero) Marshal.FreeHGlobal(output);
			}
		}

		public J2534Err ClearTxBuffer(int channelId)
		{
			try
			{
				IntPtr input = IntPtr.Zero;
				IntPtr output = IntPtr.Zero;

				return (J2534Err)_J2534DllWrapper.Ioctl(channelId, (int)Ioctl.CLEAR_TX_BUFFER, input, output);
			}
			catch (OutOfMemoryException)
			{ return J2534Err.ERR_OUT_OF_MEMORY; }

		}

		public J2534Err ClearRxBuffer(int channelId)
		{
			try
			{
				IntPtr input = IntPtr.Zero;
				IntPtr output = IntPtr.Zero;

				return (J2534Err)_J2534DllWrapper.Ioctl(channelId, (int)Ioctl.CLEAR_RX_BUFFER, input, output);
			}
			catch (OutOfMemoryException)
			{ return J2534Err.ERR_OUT_OF_MEMORY; }

		}

		public J2534Err ClearPeriodicMsgs(int channelId)
		{
			try
			{
				IntPtr input = IntPtr.Zero;
				IntPtr output = IntPtr.Zero;

				return (J2534Err)_J2534DllWrapper.Ioctl(channelId, (int)Ioctl.CLEAR_PERIODIC_MSGS, input, output);
			}
			catch (OutOfMemoryException)
			{ return J2534Err.ERR_OUT_OF_MEMORY; }

		}

		public J2534Err ClearMsgFilters(int channelId)
		{
			try
			{
				IntPtr input = IntPtr.Zero;
				IntPtr output = IntPtr.Zero;

				return (J2534Err)_J2534DllWrapper.Ioctl(channelId, (int)Ioctl.CLEAR_MSG_FILTERS, input, output);
			}
			catch (OutOfMemoryException)
			{ return J2534Err.ERR_OUT_OF_MEMORY; }

		}

		public J2534Err ClearFunctMsgLookupTable(int channelId)
		{
			try
			{
				IntPtr input = IntPtr.Zero;
				IntPtr output = IntPtr.Zero;

				return (J2534Err)_J2534DllWrapper.Ioctl(channelId, (int)Ioctl.CLEAR_FUNCT_MSG_LOOKUP_TABLE, input, output);
			}
			catch (OutOfMemoryException)
			{ return J2534Err.ERR_OUT_OF_MEMORY; }

		}

		public J2534Err AddToFunctMsgLookupTable(int channelId)
		{
			try
			{
				IntPtr input = IntPtr.Zero;
				IntPtr output = IntPtr.Zero;
				// TODO: fix this
				return (J2534Err)_J2534DllWrapper.Ioctl(channelId, (int)Ioctl.ADD_TO_FUNCT_MSG_LOOKUP_TABLE, input, output);
			}
			catch (OutOfMemoryException)
			{ return J2534Err.ERR_OUT_OF_MEMORY; }

		}

		public J2534Err DeleteFromFunctMsgLookupTable(int channelId)
		{
			try
			{
				IntPtr input = IntPtr.Zero;
				IntPtr output = IntPtr.Zero;
				// TODO: fix this
				return (J2534Err)_J2534DllWrapper.Ioctl(channelId, (int)Ioctl.DELETE_FROM_FUNCT_MSG_LOOKUP_TABLE, input, output);
			}
			catch (OutOfMemoryException)
			{ return J2534Err.ERR_OUT_OF_MEMORY; }

		}

		/// <summary>
		/// Poll for messages until we get a timeout
		/// </summary>
		/// <param name="numMsgs"></param>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public J2534Err ReadAllMessages(int channelId, int numMsgs, int timeout, out List<PassThruMsg> messages, bool readUntilTimeout = true)
		{
			messages = new List<PassThruMsg>();
			int batchSize = numMsgs; // preserve original requested count
			IntPtr rxMsgs = IntPtr.Zero;
			try
			{
				rxMsgs = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(PassThruMsg)) * batchSize);
				var m_status = J2534Err.STATUS_NOERROR;
				var m_status2 = J2534Err.STATUS_NOERROR;

				//Read the first block of messages
				m_status = PassThruReadMsgs(channelId, rxMsgs, ref numMsgs, timeout);
				if (m_status == J2534Err.STATUS_NOERROR)
				{
					var msgs = rxMsgs.AsMsgList(numMsgs);
					if (msgs.Count > 0) messages.AddRange(msgs);

					//If we are only reading this block then return now
					if (!readUntilTimeout) return m_status;
				}
				else
				{
					var msgs = rxMsgs.AsMsgList(numMsgs);
					if (msgs.Count > 0) messages.AddRange(msgs);
					//If we failed on the first read give up now
					return m_status;
				}

				//We successfully read one block, now keep going
				while (J2534Err.STATUS_NOERROR == m_status2)
				{
					numMsgs = batchSize; // reset to original batch size for each read
					m_status2 = PassThruReadMsgs(channelId, rxMsgs, ref numMsgs, timeout);
					if (m_status2 == J2534Err.STATUS_NOERROR)
					{
						var msgs = rxMsgs.AsMsgList(numMsgs);
						foreach (var msg in msgs) messages.Add(msg);
					}
					else
					{
						break;
					}
				}

				return J2534Err.STATUS_NOERROR;
			}
			catch (OutOfMemoryException)
			{ return J2534Err.ERR_OUT_OF_MEMORY; }
			finally
			{
				if (rxMsgs != IntPtr.Zero) Marshal.FreeHGlobal(rxMsgs);
			}

		}

	}

	public class J2534Exception : Exception
	{
		J2534Err _error;
		public J2534Err Error
		{
			get { return _error; }
		}
		public J2534Exception(J2534Err error)
			: base(error.ToString())
		{
			_error = error;
		}
	}



	public unsafe struct OBDX_DeviceDetails
	{
		public IntPtr DeviceName;
		public IntPtr UniqueSerial;
		public IntPtr HardwareVersion;
		public IntPtr FirmwareVersion;
		public uint ComportNumber;

		public void SetDeviceName(string val)
		{
			DeviceName = Marshal.AllocHGlobal(80);
			for (int i = 0; i < val.Length; i++) Marshal.WriteByte(DeviceName, i, (byte)val[i]);
		}
		public string GetDeviceName()
		{
			string tempval = "";
			for (int i = 0; i < 80; i++)
			{
				if (Marshal.ReadByte(DeviceName, i) == 0) { break; }
				tempval += (char)Marshal.ReadByte(DeviceName, i);
			}
			return tempval;
		}
		public void SetUniqueSerial(string val)
		{
			UniqueSerial = Marshal.AllocHGlobal(80);
			for (int i = 0; i < val.Length; i++) Marshal.WriteByte(UniqueSerial, i, (byte)val[i]);
		}
		public string GetUniqueSerial()
		{
			string tempval = "";
			for (int i = 0; i < 80; i++)
			{
				if (Marshal.ReadByte(UniqueSerial, i) == 0) { break; }
				tempval += (char)Marshal.ReadByte(UniqueSerial, i);
			}
			return tempval;
		}

		public void SetHardwareVersion(string val)
		{
			HardwareVersion = Marshal.AllocHGlobal(80);
			for (int i = 0; i < val.Length; i++) Marshal.WriteByte(HardwareVersion, i, (byte)val[i]);
		}
		public string GetHardwareVersion()
		{
			string tempval = "";
			for (int i = 0; i < 80; i++)
			{
				if (Marshal.ReadByte(HardwareVersion, i) == 0) { break; }
				tempval += (char)Marshal.ReadByte(HardwareVersion, i);
			}
			return tempval;
		}
		public void SetFirmwareVersion(string val)
		{
			//fixed (char* data = FirmwareVersion)
			//{
			//    for (int i = 0; i < val.Length; i++) *(data + i) = val[i];
			//}
			FirmwareVersion = Marshal.AllocHGlobal(80);
			for (int i = 0; i < val.Length; i++) Marshal.WriteByte(FirmwareVersion, i, (byte)val[i]);
		}
		public string GetFirmwareVersion()
		{
			string tempval = "";
			for (int i = 0; i < 80; i++)
			{
				if (Marshal.ReadByte(FirmwareVersion, i) == 0) { break; }
				tempval += (char)Marshal.ReadByte(FirmwareVersion, i);
			}
			return tempval;
		}

	}
	public struct OBDX_DeviceDetailsList
	{
		public int Count { get; set; }

		public IntPtr ListPtr { get; set; }

		public List<OBDX_DeviceDetails> GetList()
		{
			if (ListPtr == IntPtr.Zero)
				return new List<OBDX_DeviceDetails>();
			return ListPtr.AsList<OBDX_DeviceDetails>(Count);
		}
	}



	public unsafe struct PassThruMsg
	{
		public ProtocolID ProtocolID;
		public RxStatus RxStatus;
		public TxFlag TxFlags;
		public uint Timestamp;
		public uint DataSize;
		public uint ExtraDataIndex;
		public fixed byte Data[4128];

		public PassThruMsg(ProtocolID myProtocolId, TxFlag myTxFlag, byte[] myByteArray)
		   : this()
		{
			ProtocolID = myProtocolId;
			TxFlags = myTxFlag;
			SetBytes(myByteArray);
		}

		public void SetBytes(byte[] myByteArray)
		{
			// Root-cause guard for ALL callers/protocols (not CAN-FD-specific):
			// Data is a fixed byte[4128] buffer. A null array NREs here; an oversize
			// array would OVERRUN the fixed buffer into adjacent struct memory. Fail
			// loud on oversize rather than corrupt memory / marshal a bogus DataSize.
			if (myByteArray == null) { DataSize = 0; return; }
			if (myByteArray.Length > 4128)
				throw new ArgumentException($"PassThruMsg payload {myByteArray.Length} exceeds the 4128-byte Data buffer");
			DataSize = (uint)myByteArray.Length;
			fixed (byte* data = Data)
			{
				for (int i = 0; i < myByteArray.Length; i++) *(data + i) = myByteArray[i];
			}
		}

		public byte[] GetBytes()
		{
			var bytes = new byte[DataSize];
			fixed (byte* data = Data)
			{
				for (int i = 0; i < DataSize; i++) bytes[i] = *(data + i);
			}

			return bytes;
		}

		//private const string tab = "    ";

		public string ToString(string tab)
		{
			return
				string.Format(
					"{6}{5}Protocol: {0}{6}{5}RxStatus: {1}{6}{5}Timestamp: {2}{6}{5}ExtraDataIndex: {3}{6}{5}Data: {4}",
					ProtocolID,
					RxStatus,
					Timestamp,
					ExtraDataIndex,
					BitConverter.ToString(GetBytes()),
					tab,
					Environment.NewLine);
		}
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct SByteArray
	{
		public int NumOfBytes;
		public IntPtr BytePtr;

		public override string ToString()
		{
			var byteList = BytePtr.AsList<byte>(NumOfBytes);
			return BitConverter.ToString(byteList.ToArray()).Replace("-", " ");
		}
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct SConfig
	{
		public ConfigParameter Parameter;
		public uint Value; // J2534 spec requires DWORD (uint32), was incorrectly UInt16
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct SConfigList
	{
		public int Count { get; set; }

		public IntPtr ListPtr { get; set; }

		public List<SConfig> GetList()
		{
			if (ListPtr == IntPtr.Zero)
				return new List<SConfig>();
			return ListPtr.AsList<SConfig>(Count);
		}
	}
	public struct SParam
	{
		public uint Parameter; //Name of patameter
		public uint Value; //value of parameter
		public uint Supported; //support for parameter
	}
	public struct SParamList
	{
		public int Count { get; set; }

		public IntPtr ListPtr { get; set; }

		public List<SParam> GetList()
		{

			if (ListPtr == IntPtr.Zero)
				return new List<SParam>();
			return ListPtr.AsList<SParam>(Count);


		}
	}






	public static class Utils
	{
		public static string AsString(this IntPtr ptr)
		{
			return Marshal.PtrToStringAnsi(ptr);
		}

		public static IntPtr ToIntPtr(this PassThruMsg msg)
		{
			IntPtr msgPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(PassThruMsg)));
			// fDeleteOld must be FALSE on freshly-allocated (uninitialised) memory: `true` asks the
			// marshaler to DestroyStructure the garbage already at msgPtr. Benign today only because
			// PassThruMsg is fully blittable (no reference/[MarshalAs] fields); it becomes heap
			// corruption the moment any marshaled field is added to the struct.
			Marshal.StructureToPtr(msg, msgPtr, false);
			return msgPtr;
		}

		public unsafe static List<PassThruMsg> AsMsgList(this IntPtr ptr, int count)
		{
			List<PassThruMsg> list = new List<PassThruMsg>(count);
			int stride = Marshal.SizeOf(typeof(PassThruMsg));
			for (int index = 0; index < count; ++index)
			{
				IntPtr current = new IntPtr(ptr.ToInt64() + index * stride);
				list.Add(current.AsStruct<PassThruMsg>());
			}
			return list;
		}

		public static List<T> AsList<T>(this IntPtr ptr, int count) where T : struct
		{
			List<T> list = new List<T>(count);
			int stride = Marshal.SizeOf(typeof(T));
			for (int index = 0; index < count; ++index)
			{
				IntPtr ptr1 = new IntPtr(ptr.ToInt64() + index * stride);
				list.Add(ptr1.AsStruct<T>());
			}
			return list;
		}

		public static T AsStruct<T>(this IntPtr ptr) where T : struct
		{
			return (T)Marshal.PtrToStructure(ptr, typeof(T));
		}

		public static T? AsNullableStruct<T>(this IntPtr ptr) where T : struct
		{
			if (ptr == IntPtr.Zero)
				return new T?();
			return new T?((T)Marshal.PtrToStructure(ptr, typeof(T)));
		}

		public static string AsString(this List<PassThruMsg> list)
		{
			string str = string.Empty;
			for (int index = 0; index < list.Count; ++index)
				str = string.Concat(new object[4]
				{
					(object) str,
					(object) index,
					(object) " -------------------------------",
					(object) Environment.NewLine
				}) + list[index].ToString() + (object)index + " -------------------------------";
			return str;
		}
	}

	public unsafe struct SDEVICE
	{
		fixed char DeviceName[80];
		uint DeviceAvailable;
		uint DeviceDLLFWStatus;
		uint DeviceConnectMedia;
		uint DeviceConnectSpeed;
		uint DeviceSignalQuality;
		uint DeviceSignalStrength;
	}
}
//                                                                                                                                                                                                                                                                                                                                                                                      
//
//                                                                                                                                                ...                                         
//                                                     .-=*%%%%%%%%%%%%%%%%%%%#%###%#+-.                                                     ....:::::.....                                   
//                                        .-*#%#*+++=====+++++++++++*##%###**+++===------------:                           .............     .::---------::..                                 
//                                -#**+++++++*#  ====+++=:.                              .+*%@@@@@ .:                   ........:::...........-------+%%#---:..                               
//                           :%%*-...   ...=*++++         :=+=                ...:. +##%@@@*-**#%@@@@::-.              .....:--=+*#@@@%+=--::::----------::....                               
//                        =@= .    ..- ... :.-*==:  .=:...:::::::::::::::  ..   ::-.:@@@@@@@@+.#%%@@@@@@ :=           ....:#%%%+=-=++=-:.-*@@@@@@@@@@@@@@%##*:#                               
//                      *-   :----.:::.:::-: :.+==.  %@@@@@@@@@@@@#::..              =@@@@@@%*:-=#%@@@@@@@* --         .....::-+%@@@@@%#+-:..................*=.-+                            
//                    #.. ----===.- - :------.:.+-=   ..:+*+=:.                  ....= .   .. .====#@@@@@@@@@  +.       ...@@@@=:......:::.....................-+..%                          
//                  :=.. =-=====--.- :-%%%#*+- ..=-=   @@@@#*+                   ==     ..  .. .=====#@@#@@@@@@: --      .%#.:@...:...::::..:...................:--.-=                        
//                 #..%::-=+#@@@--.- -#+-----::..:--+                           .   .   ...  :  .======#@#%@@%@@@= := -++-.  :@ .................................:-:: % *@@@                  
//               :=.=%*.=+@@%%#--.-.:=#-------:.: =-=- .              .-- =    .**  -    ..   :  :======+#%+@@%%%@@* .+::++++@  .........................    ..        +*.                    
//              +:.=%%..+@@@@@@--.- -+#--------- ..+-=  .              . :=++.-##*-**     ..   .  -=========:+    .:-   *::++:.   .....        ..-==+++-::..:=+*#%%@@@@%:-:#@:                
//        .-+*.*-: %@#.=+%+==--:.:: -#*...  .:--.  -=-*           ....:......:**+=-:   .:--::...             ..:-===-:.   ...         ..:+-----=*@@@@@%+*@@@@=%#@@@#...-=.....+%              
//   ##+-..  :#*..:#%- +*@=---.- -  #%%*==---%..:   +--:      ..........              :=*%@@@@@@@@@@@@@@@@@@@@@@@%%%%%%%@@@@@:-=++=.   ....=@@@=@@@@+:*=...............:+::::::.@-            
//   ..+.   #*++..-*+- =%@==--.- .-------=++#@@..:  ==:-+.           ...::--------*=:..........:+**************************###%@#:===*#+==:   ........................:::---=@@@%@=           
//   ... .#*+++=.:.:..    ....   :***********+*+ .  .++--=+=:-==--=+*##+=====*%%%*+++===-:..........:========+++++++++++++++**####%*+%@@@%=-+*%#.  ..............      -#@@@@@@@@=%%          
//    .#%%*++++=++++===+====---..            .-     .:++=----=++=-:#%#****+++==--=+%@@%*=========-:.....========+++++++++++++++++***#%#*##%@#-=*+*%@@.     ..-.:..     -@@@@@#+=*@@@.         
//   .++++%%%%=#%%%%%%#***+++=-==-..                .::====-------=++=.-*#++=======-----+%@%*+==========-..======+++=++++++++++++++++++++++**#*-+%++++#@= . .     ..-..***-+#@@@@@@@*         
//   =--:=%%%%-=...=======++*+%%##*+++++=++=---......::-==++++=--------=+=:.++=====---------=+@@%=====================++++++++++++++++++++++++++=-%#+++++*%  ...-=+**###+#@@@@@@@@@@@         
//   --:==-+%%-.....=========-=...===++#%#+=---:...:::::====+++++==--------==-.-==--=======------+%@*===============++++++++++++++++++++++++++++++-=+++++= -*         ::...=@@#+@@#-@         
//   -.-:-###-.-...::::::---:..:--:===========+##==:::--======++=======-------==-.---============----*%%=============+++++++++***+=----=++++++++++++*%:.-=@@#@ :***%  ..    @-:@@@@*=.        
//   .*:-%###+::-=*##***++==-::...::-----::------.--=*#%%%%%=============--------==.:===============----+%#+=====+=====-=++++++**##%%%%%%###*+=--:....%@@@@@@#@ .=+#:      *%. :***#@         
//   =::-::::=-.::-=++++*####+#*******++=-:::::::..-===--%%%%%=====--======.                   ....:-====--=-=+=+++++**##+-..                     ..-+.@@@@@*=*:          %%:@@@@@#=+         
//   #::.   ..::.:.....................:-==-::-=+.::::-**-::::=**=-----====...+@+*@@%  #%@*=%:...*+-=#@-..-:-:..                :.=+*+*:          ..=+ ==#:::-  * :#%%*+=--:. ....            
//   *+. . :- ...:.................................=*=*::.   .:::=#-----====...##*#@@.=-@-@@@@:@@#@##@*@@-: ==%.              @ .@:.#+.=          ..++.   :. .  %%                            
//   *= .   +-  -.:............::::::::::::::::::...:-.    ...   ::+*----==:.  .+%@@@@ %:.:@@@ -@@@@@@@@@#=-.:-+%               .             ..::-*++++++-.#*+#@%-                           
//   .. +    %=. *.:.:::::::::.::::::::::::::......:..      .   . .--%--.--====---:      =@@.  ..@+@@@@@#::::. =-*%                 ..:-::::::+%@@*+++++****#%++=++:                          
//   .. .     -*  .................................:.. ...     % .  ..#----+=::-==:::=*=.   ...    =**- ..  .:..-==%%=----------====++****##%%@@@@%#**++++++++++++%#.    ..-+#*=-:.           
//   .. .     %.: .................................:.  +        :#.. .:#%#*:::..----#%%%========-----.   .=+++++++++++*####***+++==-:....-*@@@@@-++++++++++++++#*..    .:=++-.............    
//    . -    :  .  ............ ...................:. -          .#.- .:#-:-:....:-----%%+--====+++++++++++++++++++++++**++.#@@@=:--@@@@@@@@@@@@.++=-:.-++++=-. ::%-  .....    ....:..        
//    . =    : :    ................................              +-:- .::--=-....-----%@@*-----------======+%%%%#*@%**++++-:@@@@@@%%@@@@@%@@%%@....    . ::.. @@@@=.      .:=#@%=.           
// ..-.   :.. .:       ......... ................... +             @   ...-##::...----------------------=========++++===-::. @@@%@@%#%%@@@@@@@@@=         ..==.#@@=   ..                      
//   .   :     .    +=-:###-.  ..........     ..... .-         .    = . .........:-:-*%%%%%@@@@@@@@@@@@@@#*++====:  ......   @+@@@@@@@@@@@#=..             . ==:....  .                       
// ......... .           ..:-+-....:-+*+-.  .............-         -    # .  .......------====-:::......      ..=+=.                                            . ++#%%%+                         
// .......... +    .     .........=*+-......-+*+:.  .... #  :::.  .@.   = ..  .....--::::.  .#@#**#   ...::=+***=.                                             ..:+:-#@+..                        
// ......  =           .............:---............ @.        .  ..:  .  ....-.....  .:@@@@@@@@ ..:-=*###+:                                    ...  ... .-+**@@-....  .                      
//  ....... :::                      ............... +.            ..  ..  .......     ..:*@@@@@ ..:----=+.                           ...::::::.:-=#@@@#=.....                                
//    ......                                   ...... @=            :      ...........  ..%@@@  ---:---++                ....:::::::.::=#@@@@@@*-:....                                        
//                                                    .*= .         .       .=%%#+=-==-----:::-----==+++%#:.       ......-==++*##%%#+-:......                                                 
//                                                     ==.                  ........:-+#@@@@@@@#+++++++++++++====--::..............                                                           
//                              ......                   #.                  ......................................                                                                           
//                                     ........:...:.     ++     :            .....................                                                      ....                                 
//                                         ..:-==:::-=:                                                                                          ..                                           
//                                                  .:--:                                                                           ...                                                       
//                                                                                                          ...........                                                                       
//                                                                                                                                                                                            
//                                                                                                                                                                                            
//                                                                                         .........                                                                                          
//                                                                                                                                                  
