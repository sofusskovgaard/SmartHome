//
// Created by Sofus Skovgaard on 02/05/2022.
//

#include "LedHelper.h"

void LedHelper::led_on() {
    PinStatus _status = digitalRead(LED_BUILTIN);
    if (_status != PinStatus::HIGH) {
        digitalWrite(LED_BUILTIN, HIGH);
    }
}

void LedHelper::led_off() {
    PinStatus _status = digitalRead(LED_BUILTIN);
    if (_status != PinStatus::LOW) {
        digitalWrite(LED_BUILTIN, LOW);
    }
}

void LedHelper::toggle_led() {
    PinStatus _status = digitalRead(LED_BUILTIN);
    if (_status == PinStatus::HIGH) {
        led_off();
    } else {
        led_on();
    }
}

void LedHelper::strobe_led(unsigned long duration) {
    unsigned long startMs = millis();
    unsigned long lastChange = millis();

    while (lastChange-startMs <= duration) {
        unsigned long ms = millis();
        if (ms-lastChange >= 25) {
            lastChange=ms;
            toggle_led();
        }
    }

    led_off();
}

void LedHelper::configure() {
    pinMode(LED_BUILTIN, OUTPUT);
}
