//
// Created by Sofus Skovgaard on 03/05/2022.
//

#include <Arduino.h>
#include "ServoManager.h"
#include "helpers/PrintHelper.h"

Servo ServoManager::_servo = Servo();
int ServoManager::_pos = 0;

void ServoManager::open() {
    for (_pos = 0; _pos < 90; _pos += 1) {
        _servo.write(_pos);
        delay(5);
    }
}

void ServoManager::close() {
    for (_pos = 90; _pos > 0; _pos -= 1) {
        _servo.write(_pos);
        delay(5);
    }
}

void ServoManager::configure() {
    Serial.println(PrintHelper::ts() + "[Servo] Configuration initiated");
    _servo.attach(5);
    _servo.write(_pos);
}

void ServoManager::toggle() {
    if (_pos == 0) {
        open();
    } else {
        close();
    }
}
