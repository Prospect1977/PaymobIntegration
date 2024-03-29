

function PleaseWait(action, direction, message) {
    var textMessage = "Please Wait";
    
    if (direction == null) {
        direction = "ltr";
    }
    if (direction == "rtl"){
        textMessage = "برجاء الانتظار";
    }

    if (action == 'show') {
        if (message != null) {
            textMessage = message;
        } 
        $('#pleaseWait').detach();
        var element = $(`<div id="pleaseWait" style="direction:${direction};margin:auto;position:absolute;left:50%;border-radius:7px;border:1px solid lightgray;vertical-align:middle;padding:10px;box-shadow: 0 0 10px #ccc4cc;z-index: 100;background-color: white">
            <img src="../../images/PleaseWait_Gray.gif" /><span style="margin-left:5px">${textMessage }</span>
        </div>`).appendTo('body').show();
        element.css("top", $(window).scrollTop() + $(window).height() / 2);
    }

    if (action == 'hide') {
        $('#pleaseWait').detach();
    }
}
