//
// Created by Sofus Skovgaard on 03/05/2022.
//

#include "PrintHelper.h"

String PrintHelper::ts() {
    String _str = "[";
    _str.concat(RTCManager::get_rtc().now().timestamp(DateTime::TIMESTAMP_FULL).c_str());
    _str.concat("]");
    return _str;
}
