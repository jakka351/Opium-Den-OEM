<a href="https://tester.engineering"><img width="24%" height="24%" align="right" alt="RightToRepair" src="https://github.com/user-attachments/assets/b41c7c2a-f293-45aa-97d7-bebcd65e9433" /></a>

# Opium-Den-OEM
Chinese Vehicle Open Tooling - **Data Dump and Community Tooling Bootstrapper**  

It's not that hard to build some tooling for these vehicles, here's some raw data to help. I will post some foundations for something in the coming weeks.  
*Jakka*

## Challenge - can the open source community build an automotive grade diagnostic tool?  
Discussions are open - the data is dropped, what can the community build? Fork the repo if you want to help. The data is actually irrelevant to the viability of achieving this goal, the real core is very simple - the on the wire protocols that automotive systems use, they are widely documented via SAE and ISO standards - but one size does not fit all - you have need a system that is protocol-era-aware and protocol-dialect-proficient. Its a universal language but there are hundreds of dialects and language nuances to grasp and control.   

## Bytes on the wires  
At the core the system is architecture is simple - 4 protocols layered one over the other and you have everything you need.    
  
    [PC] -> SAE J2534(PassThru) --> ISO 14229(UDS) --> ISO 15765(ISO-TP) --> ISO 11898(CAN) -> [ECU]   
  
You build everything around these four and you are on the money. Understanding all of them is crucial to building this. 
