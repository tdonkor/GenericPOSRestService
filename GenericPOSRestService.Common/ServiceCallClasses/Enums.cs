using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace GenericPOSRestService.Common.ServiceCallClasses
{
    public enum StatusResponseError : int
    {
        POSError = 1,
        Http401 = 2,
        Http404 = 3,
        Http500 = 4
    }

    public enum OrderResponseError : int
    {
        POSError = 1,
        Http401 = 2,
        Http403 = 3,
        Http404 = 4,
        Http500 = 5
    }

    public enum OrderResponsePOSError : int
    {
        TerminalNotReady = 1,
        ItemNotFound = 2,
        TenderNotFound = 3,
        OrderIDNotFound = 4
    }

    public enum TestDiagResponsePOSError : int
    {
        ErrorCallingMethod = 1,
        ErrorSelectingOrderModes = 2,
        EatInCodeNotFound = 3,
        TakeAwayNotFound = 4
    }

    public enum Closed : int
    {
        Open = 0,
        Closed = 1
    }

    public enum Location : int
    {
        EatIn = 0,
        TakeAway = 1
    }

    public enum Status : int
    {
        OrderStarted = 0,
        OrderRefresh = 1,
        OrderTotalized = 2,
        OrderPaid = 3,
        OrderCanceled = 6
    }

    public enum FunctionNumber
    {
        [Description("Send to POS a complete order")]
        EXT_COMPLETE_ORDER = 3,
        [Description("Send to POS an open order")]
        EXT_OPEN_ORDER = 4,
        [Description("Close on POS last order opened in preview session of integrator")]
        EXT_TENDER_ORDER = 5,
        [Description("Void on POS last order opened in preview session of Integrator")]
        EXT_VOID_ORDER = 6,
        [Description("Unlock on POS last order opened in preview session of Integrator")]
        EXT_UNLOCK_ORDER = 32,
        [Description("Pre calculate")]
        PRE_CALCULATE = 33
    }

    public enum Errors : int 
    { 
        POSError = -1,
        OrderNotFound = -2,
        ItemListNotSpecified = -3,
        ItemNotFound = -4,
        ErrorDeserializeRequest = -5,
        KioskNotSpecified = -6,
        RefIntNotSpecified = -7,
        TenderItemListNotSpecified = -8,
        TenderNotFound = -9,
        OrderIDNotSpecified = -10,
        TerminalNotReady = -11,
        ThereIsAnOrderWithKioskAndRefInt = -12,
        CannotDoActionOnOrderBecauseIsNotOpen = -13,
        MethodNotSpecified = -14,
        CultureNameNotSpecified = -15,
        ErrorCallingPOSMethod = -16,
        JsonIncorrectFormat = -17,
        OrderMissing = -18
    }
}
