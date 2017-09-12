$(document).ready(function () {

    var height1 = $('.dashboard-map-image').innerHeight();
    var height2 = $('.dashboard-map-image').innerHeight();
    $('.admin-box-content').css('min-height', height1);
    $('.dashboard-device-home').css('min-height', height2);

    $('.admin-dashboard-1-toggle').click(function () {
        $(this).toggleClass("value-on");
        if ($(this).hasClass("value-on")) {
            $('#txtPower').val("off")
        } else {
            $('#txtPower').val("on")
        }
        $("#formControll").submit();
    });

    $('#cbxAirGapOn').click(function () {
        if ($(this).is(':checked')) {
            $("#cbxAirGapOnStatus").val("on");
        } else {
            $("#cbxAirGapOnStatus").val("off");
        }
        $('#fmAirGapOn').submit();
    });

    $('#btnSignin').click(function (event) {
        var email = $('#txtEmail').val();
        var password = $('#txtPassword').val();

        if (!email) {
            $('#divResultEmailSignInText').text('Email is required');
            $('#divResultEmailSignIn').show('fade');
            event.preventDefault();
        } else {
            $('#divResultEmailSignIn').hide('fade');
        }

        if (!password) {
            $('#divResultPasswordText').text('Password is required');
            $('#divResultPassword').show('fade');
            event.preventDefault();
        } else {
            $('#divResultPassword').hide('fade');
        }
    })

    //$('#btnSignUp').click(function (e) {
    //    var firstName = $('#txtFirstName').val();
    //    var lastName = $('#txtLastName').val();
    //    var email = $('#txtEmailSignUp').val();

    //    if (!email) {
    //        $('#divResultEmailSignUpText').text('Email is required');
    //        $('#divResultEmailSignUp').show('fade');
    //        event.preventDefault();
    //    } else {
    //        $('#divResultEmailSignUp').hide('fade');
    //    }

    //    if (!firstName) {
    //        $('#divResultFirstNameSignUpText').text('Password is required');
    //        $('#divResultFirstNameSignUp').show('fade');
    //        event.preventDefault();
    //    } else {
    //        $('#divResultFirstNameSignUp').hide('fade');
    //    }

    //    if (!lastName) {
    //        $('#divResultLastNameSignUpText').text('Password is required');
    //        $('#divResultLastNameSignUp').show('fade');
    //        event.preventDefault();
    //    } else {
    //        $('#divResultLastNameSignUp').hide('fade');
    //    }
    //});
	
	$('.navbar-toggle').click(function () {
		$('.admin-menu').addClass("active-menu");
	});
	$(".bg-mobi").click(function () {
		$('.admin-menu').removeClass("active-menu");
	});
	$(".btn-close-menu").click(function () {
		$('.admin-menu').removeClass("active-menu");
    });

    function turnOnOrOfAirGap(action) {
        this.form.submit();
    }

    function gup(name) {
        name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
        var regexS = "[\\?&]" + name + "=([^&#]*)";
        var regex = new RegExp(regexS);
        var results = regex.exec(window.location.href);
        if (!results)
            return "";
        else
            return results[1];
    }

    var accountid = gup('token');
    $('#hdToken').val(accountid);

    $('#btnPasswordChange').click(function () {
        var existingPassword = $('#ExistingPassword').val();
        var newPassword = $('#NewPassword').val();
        var reTypePassword = $('#ReTypePassword').val();

        if (!newPassword) {
            $('#divResultNewPasswordText').text('New Password is required');
            $('#divResultNewPassword').show('fade');
            return false;
        } else if (newPassword.length < 6) {
            $('#divResultNewPasswordText').text('Your New Password must be at least 6 characters ');
            $('#divResultNewPassword').show('fade');
            return false;
        } else {
            $('#divResultNewPassword').hide('fade');
        }

        if (!reTypePassword) {
            $('#divResultReTypePasswordText').text('ReType Password is required');
            $('#divResultReTypePassword').show('fade');
            return false;
        } else {
            $('#divResultExistingPassword').hide('fade');
        }

        if (newPassword != reTypePassword) {
            $('#divResultNewPasswordText').text("New Password and ReType Password don't match");
            $('#divResultNewPassword').show('fade');
            return false;
        }

        $("#frmChangePassword").submit();
    });

    $('#btnSetPassword').click(function () {
        var password = $('#txtPassword').val();
        var confirmpassword = $('#txtConfirmPassword').val();

        if (!password) {
            $('#divResultPasswordText').text('Password is required');
            $('#divResultPassword').show('fade');
            return false;
        } else if (password.length < 6) {
            $('#divResultPasswordText').text('Your password must be at least 6 characters ');
            $('#divResultPassword').show('fade');
            return false;
        } else {
            $('#divResultPassword').hide('fade');
        }

        if (!confirmpassword) {
            $('#divResultConfirmPasswordText').text('ConfirmPassword is required');
            $('#divResultConfirmPassword').show('fade');
            return false;
        } else if (confirmpassword.length < 6) {
            $('#divResultConfirmPasswordText').text('Your password must be at least 6 characters ');
            $('#divResultConfirmPassword').show('fade');
            return false;
        }else {
            $('#divResultConfirmPassword').hide('fade');
        }

        if (password !== confirmpassword) {
            $('#divResultText').text("The confirm password and new password don't match");
            $('#divResult').show('fade');
            return false;
        } else {
            $('#divResultConfirmPassword').hide('fade');
        }

        $.ajax({
            type: 'POST',
            url: '/Login/CreatePassword',
            data: {
                token: $('#hdToken').val(),
                password: $('#txtPassword').val(),
                confirmPassword: $('#txtConfirmPassword').val(),
            },
            success: function (response) {
                if (response.responseText === 'Success') {
                    //window.location.href = response.url;
                    $('#ModalPasswordSuccess').modal("show");
                } else {
                    $('#divResult').text(response.responseText).removeClass('alert-success');
                    $('#divResult').text(response.responseText).addClass('alert-danger');
                    $('#divResult').show('fade');
                }
                //$('#divResult').show('fade');
            },
            error: function (jqXHR) {
                $('#divResultText').text(jqXHR.responseText)
                $('#divResult').text(response.responseText).removeClass('alert-success');
                $('#divResult').text(response.responseText).addClass('alert-danger');
                $('#divResult').show('fade');
            }
        });
    }); 

    $("#ddlApplianceUser").change(function () {
        //alert($(this).val());
        if (this.value != 0) {
            this.form.submit();
        }
    });

    $("#ddlAppliance").change(function () {
        //alert($(this).val());
        if (this.value != 0)
        {
            this.form.submit();
        }
    });

    $("#ddlApplianceAirGap").change(function () {
        //alert($(this).val());
        if (this.value != 0) {
            this.form.submit();
        }
    });


    $('#btnUpdateNotificationPreference').click(function () {
        var selected = [];
        $('.table-responsive input:checked').each(function () {
            selected.push($(this).attr('value'));
        });

        if (selected.length > 0) {
            $('#hdValue').val(selected.join());
        }
    });

    //$('input[type=checkbox]').click(function () {
    //    if ($(this).is(':checked')) {
    //        $('#cbxNotificationStatus').val("on");
    //        $('#cbxAccountId').val($(this).val());
    //    } else {
    //        $('#cbxNotificationStatus').val("off");
    //        $('#cbxAccountId').val($(this).val());
    //    }
    //    //$('#fmNotificationPreference').submit();
    //});

    function validateEmail(email) {
        var EMAIL_PATTERN = "^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@"
            + "[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$";
        return email.match(EMAIL_PATTERN);
    }

    $('#btnForgotPassword').click(function () {
        var email = $('#txtEmail').val();
        if (!email) {
            $('#divResultText').text('Email is required');
            $('#divResult').show('fade');
            return false;
        } else {
            $('#divResult').hide('hide');
        }

        //if (!validateEmail(email)) {
        //    $('#divResultText').text('Email is not valid');
        //    $('#divResult').show('fade');
        //    return false;
        //} else {
        //    $('#divResult').hide('fade');
        //}

        $.ajax({
            type: 'POST',
            url: '/Login/ForgotPassword',
            data: {
                email: $('#txtEmail').val()
            },
            success: function (response) {
                if (response.responseText === 'Success')
                {
                    $('#ModalPasswordReset').modal("show");
                } else {
                    $('#divResult').text(response.responseText).removeClass('alert-success');
                    $('#divResult').text(response.responseText).addClass('alert-danger');
                    $('#divResult').show('fade');
                }
                //$('#divResult').show('fade');
            },
            error: function (jqXHR) {
                $('#divResultText').text(jqXHR.responseText)
                $('#divResult').text(response.responseText).removeClass('alert-success');
                $('#divResult').text(response.responseText).addClass('alert-danger');
                $('#divResult').show('fade');
            }
        });
    })

});

$('.form_date1').datetimepicker({
    language:  'us',
    weekStart: 1,
    todayBtn:  1,
	autoclose: 1,
	todayHighlight: 1,
	startView: 2,
	minView: 2,
	forceParse: 0,
    orientation: "left",
    endDate: new Date(),
    startDate: new Date(new Date().setDate(new Date().getDate()  - 30))
});
$('.form_date2').datetimepicker({
    language:  'us',
    weekStart: 1,
    todayBtn:  1,
	autoclose: 1,
	todayHighlight: 1,
	startView: 2,
	minView: 2,
	forceParse: 0,
    orientation: "left",
    endDate: new Date(),
    startDate: new Date(new Date().setDate(new Date().getDate() - 30))
});

// initialize and setup facebook js sdk
window.fbAsyncInit = function () {
    FB.init({
        appId: '1295308777253761', // 1845874455679492 airgap
        xfbml: true,
        version: 'v2.9'
    });
    //FB.getLoginStatus(function (response) {
    //    if (response.status === 'connected') {
       
    //    } else if (response.status === 'not_authorized') {
    //        document.getElementById('status').innerHTML = 'We are not logged in.'
    //    } else {
    //        document.getElementById('status').innerHTML = 'You are not logged into Facebook.';
    //    }
    //});
};
(function (d, s, id) {
    var js, fjs = d.getElementsByTagName(s)[0];
    if (d.getElementById(id)) { return; }
    js = d.createElement(s); js.id = id;
    js.src = "//connect.facebook.net/en_US/sdk.js";
    fjs.parentNode.insertBefore(js, fjs);
}(document, 'script', 'facebook-jssdk'));

// login with facebook with extra permissions
function loginfacebook() {
    FB.login(function (response) {
        if (response.status === 'connected') {
            createUserAndLoginWithUserFacebook()
        } else if (response.status === 'not_authorized') {
            document.getElementById('status').innerHTML = 'We are not logged in.'
        } else {
            document.getElementById('status').innerHTML = 'You are not logged into Facebook.';
        }
    }, { scope: 'email' });
}

function createUserAndLoginWithUserFacebook() {
    $("#LoadingImage_signin").show();
    $("#LoadingImage_signup").show();
    FB.api('/me', 'GET', { fields: 'first_name,last_name,email,id' }, function (response) {
        console.log(response);
        $.ajax({
            type: 'POST',
            url: '/Login/CreateUserWithUserFacebook',
            data: {
                firstname: response.first_name,
                lastname: response.last_name,
                email: response.email,
                appId: response.id
            },
            success: function (response) {
                $("#LoadingImage_signin").hide();
                $("#LoadingImage_signup").hide();
                if (response.responseText === 'Success') {
                    window.location.href = response.url;
                } else {
                    alert(response.error)
                }
            }
        });
    });
}

function deleteSerialNumber(id) {
    $("#LoadingImage-fullWidth").show();
    $.post('Air/DeleteSerialNumber', { id: id }, function (data) {
        if (data.success) window.location.reload();
    });
}