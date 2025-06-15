
const splt1 = document.querySelector('#split-chnl-main');
const splt2 = document.querySelector('#split-main-usr')
let splt1_pos = 0;
let splt2_pos = 20;
function ToggleChats() {
    if(splt1.position < 5){
        //splt2.position = 0;
        splt1.position = splt1_pos;
    } else {
        splt1_pos = splt1.position;
        splt1.position = 0;
    }
}
function ToggleUsers() {
    if(splt2.position < 5){
        //splt1.position = 0;
        splt2.position = splt2_pos;
    } else {
        splt2_pos = splt2.position;
        splt2.position = 0;
    }
}
function OpenChatsMobile() {
    splt2.position = 0;
    splt1.position = 80;
    $(".header-mob").hide();
    $("#channel-name").hide();
    $("#close-mob").show();
}
function OpenUsersMobile() {
    splt1.position = 0;
    splt2.position = 80;
    $(".header-mob").hide();
    $("#channel-name").hide();
    $("#close-mob").show();
}
function CloseCiUMobile(){
    splt1.position = 0;
    splt2.position = 0;
    $(".header-mob").show();
    $("#channel-name").show();
    $("#close-mob").hide();
}
function handleResponsiveElements(stat) {
    const elementsToHide = document.querySelectorAll('.header-circle');
    const elementsToShow = document.querySelectorAll('.header-mob');

    if (window.innerWidth < 700) {
        if (stat){
            splt1.position = 0;
            splt2.position = 0;
        }
        elementsToHide.forEach(el => el.style.display = 'none');
        elementsToShow.forEach(el => el.style.display = '');
    } else {
        if (stat){
            splt1.position="25";
            splt2.position="25";
        }
        //$("#img[slot=divider]").hide();
        elementsToHide.forEach(el => el.style.display = '');
        elementsToShow.forEach(el => el.style.display = 'none')
    }
}

window.addEventListener('DOMContentLoaded', () => handleResponsiveElements(true));
window.addEventListener('resize', () => handleResponsiveElements(false));


$(function() {
    $("#send-msg-txt").on("keydown", function(e) {
        if (e.key === "Enter") {
            e.preventDefault();
            $("#send-msg-btn").click();
        }
    });
});



    /*if (chatVisible) {
        $('#glob-servers').animate({ width: '0', opacity: 0 }, 300, function() {
            $(this).hide();
        });
        $('#glob-chats').animate({ width: '0', opacity: 0 }, 300, function() {
            $(this).hide();
        });
    } else {
        $('#glob-servers').show().animate({ width: '90px', opacity: 1 }, 300);
        $('#glob-chats').show().animate({ width: '300px', opacity: 1 }, 300);
    }
    chatVisible = !chatVisible;*/


    /*if (usersVisible) {
        $('#glob-users').animate({ width: '0', opacity: 0 }, 300, function() {
            $(this).hide();
        });
    } else {
        $('#glob-users').show().animate({ width: '300px', opacity: 1 }, 300);
    }
    usersVisible = !usersVisible;*/