#!/usr/bin/env python
#coding: utf8
import lirc
import time
import requests
import os
import RPi.GPIO as GPIO

sockid=lirc.init("dev", blocking = False)

GPIO.setmode(GPIO.BCM)
GPIO.setwarnings(False)
GPIO.setup(21,GPIO.OUT)

tvfifo = "tvcom"
comfifo = "/home/pi/SmartHouseServer/comfifo"

while True:
    codeIR = lirc.nextcode()
    if codeIR != []:
        
        GPIO.output(21,GPIO.HIGH)
        print codeIR[0]
        if "KEY_VOLUMEUP" in codeIR[0]:
            requests.get('http://127.0.0.1:8081/api/SmartHouse/VolumeUp')
        elif "KEY_VOLUMEDOWN" in codeIR[0]:
            requests.get('http://127.0.0.1:8081/api/SmartHouse/VolumeDown')
        elif "KEY_FORWARD" in codeIR[0]:
            requests.get('http://127.0.0.1:8081/api/SmartHouse/Next')
        elif "KEY_REWIND" in codeIR[0]:
            requests.get('http://127.0.0.1:8081/api/SmartHouse/Prev')
        elif "KEY_POWER" in codeIR[0]:
            requests.get('http://127.0.0.1:8081/api/SmartHouse/Power')
        elif "KEY_PLAY" in codeIR[0]:
            requests.get('http://127.0.0.1:8081/api/SmartHouse/Play')
        elif "KEY_PAUSE" in codeIR[0]:
            requests.get('http://127.0.0.1:8081/api/Remote/Pause')
        elif "KEY_STOP" in codeIR[0]:
            requests.get('http://127.0.0.1:8081/api/Remote/Stop')
        elif "KEY_UP" in codeIR[0]:
            requests.get('http://127.0.0.1:8081/api/Remote/Up')
        elif "KEY_DOWN" in codeIR[0]:
            requests.get('http://127.0.0.1:8081/api/Remote/Down')
        elif "KEY_LEFT" in codeIR[0]:
            requests.get('http://127.0.0.1:8081/api/Remote/Left')
        elif "KEY_RIGHT" in codeIR[0]:
            requests.get('http://127.0.0.1:8081/api/Remote/Right')
        elif "KEY_OK" in codeIR[0]:
            requests.get('http://127.0.0.1:8081/api/Remote/Ok')
        elif "KEY_FN_F1" in codeIR[0]:
            requests.get('http://127.0.0.1:8081/api/SmartHouse/Pandora')
        elif "KEY_FN_F2" in codeIR[0]:
            requests.get('http://127.0.0.1:8081/api/SmartHouse/Music')
        elif "KEY_FN_F3" in codeIR[0]:
            requests.get('http://127.0.0.1:8081/api/SmartHouse/XBox')
        elif "KEY_FN_F4" in codeIR[0]:
            requests.get('http://127.0.0.1:8081/api/SmartHouse/TV')
        elif "KEY_KP1" in codeIR[0]:
            requests.get('http://127.0.0.1:8081/api/Remote/Love')
        elif "KEY_KP2" in codeIR[0]:
            requests.get('http://127.0.0.1:8081/api/Remote/Ban')
        elif "KEY_KP3" in codeIR[0]:
            requests.get('http://127.0.0.1:8081/api/Remote/TiredOfThisSong')
        elif "KEY_KP4" in codeIR[0]:
            requests.post('http://127.0.0.1:8081/api/SmartBlub/Toggle')
        elif "KEY_KP5" in codeIR[0]:
            requests.post('http://127.0.0.1:8081/api/SmartBlub/SetWhite')
        elif "KEY_KP6" in codeIR[0]:
            requests.post('http://127.0.0.1:8081/api/SmartBlub/SetRed')        
        elif "KEY_KP9" in codeIR[0]:
            requests.post('http://127.0.0.1:8081/api/Sensor/ToogleAirCondition')
        else:
            requests.get('http://127.0.0.1:8081/api/SmartHouse/TVCommand?c=' + codeIR[0])
            
    time.sleep(0.01)
    GPIO.output(21,GPIO.LOW)

