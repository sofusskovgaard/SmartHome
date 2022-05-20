//
// Created by Sofus Skovgaard on 03/05/2022.
//

#include "SenderCommandBase.h"

SenderCommandBase::SenderCommandBase() {
    collected_at = RTCManager::get_rtc().now();
}

std::string SenderCommandBase::topic() {
    return "";
}
