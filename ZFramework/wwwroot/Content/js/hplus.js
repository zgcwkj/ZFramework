//导航切换
function NavToggle() {
    $(".navbar-minimalize").trigger("click");
}
//菜单切换过度动画
function SmoothlyMenu() {
    $("body").hasClass("mini-navbar") ? $("body").hasClass("fixed-sidebar") ? ($("#side-menu").hide(), setTimeout(function () {
        $("#side-menu").fadeIn(500);
    }, 300)) : $("#side-menu").removeAttr("style") : ($("#side-menu").hide(), setTimeout(function () {
        $("#side-menu").fadeIn(500);
    }, 100));
}
//本地存储支持
function localStorageSupport() {
    return "localStorage" in window && null !== window.localStorage;
}
//前台菜单生成
function MenuGenerate(listData) {
    var outHrml = "";//输出到页面
    for (var i = 0; i < listData.length; i++) {
        if (listData[i].parent_id == 0) {
            var TitleA = listData[i].title;//一级菜单名称
            var IconA = listData[i].icon;//一级菜单图标
            var LinkA = listData[i].link;//一级菜单连接
            //开始准备
            outHrml += "<li>";
            if (LinkA.trim() == "") {
                outHrml += "<a href='javascript:void(0);'>";
                if (IconA != "") outHrml += "<i class='" + IconA + "'></i>";
                outHrml += "<span class='nav-label'>" + TitleA + "</span>";
                outHrml += "<span class='fa arrow'></span>";
                outHrml += "</a>";
                for (var ii = 0; ii < listData.length; ii++) {
                    if (listData[i].menu_id.toString() == listData[ii].parent_id.toString()) {
                        var TitleB = listData[ii].title;//二级菜单名称
                        var IconB = listData[ii].icon;//二级菜单图标
                        var LinkB = listData[ii].link;//二级菜单连接
                        outHrml += "<ul class='nav nav-second-level'>";//ul
                        outHrml += "<li>";//li
                        outHrml += "<a class='J_menuItem' href='" + LinkB + "'>";
                        if (IconB != "") outHrml += "<i class='" + IconB + "'></i>";
                        outHrml += "<span class='nav-label'>" + TitleB + "</span>";
                        outHrml += "</a>";
                        outHrml += "</li>";//li
                        outHrml += "</ul>";//ul
                    }
                }
                outHrml += "";
            }
            else {
                outHrml += "<a class='J_menuItem' href='" + LinkA + "'>";
                if (IconA != "") outHrml += "<i class='" + IconA + "'></i>";
                outHrml += "<span class='nav-label'>" + TitleA + "</span>";
                outHrml += "</a>";
            }
            outHrml += "</li>";
        }
    }
    //添加 HTML
    document.getElementById("side-menu").innerHTML += outHrml;

    //计算元素集合的总宽度
    function calSumWidth(elements) {
        var width = 0;
        $(elements).each(function () {
            width += $(this).outerWidth(true);
        });
        return width;
    }
    //滚动到指定选项卡
    function scrollToTab(element) {
        var marginLeftVal = calSumWidth($(element).prevAll()), marginRightVal = calSumWidth($(element).nextAll());
        // 可视区域非tab宽度
        var tabOuterWidth = calSumWidth($(".content-tabs").children().not(".J_menuTabs"));
        //可视区域tab宽度
        var visibleWidth = $(".content-tabs").outerWidth(true) - tabOuterWidth;
        //实际滚动宽度
        var scrollVal = 0;
        if ($(".page-tabs-content").outerWidth() < visibleWidth) {
            scrollVal = 0;
        } else if (marginRightVal <= (visibleWidth - $(element).outerWidth(true) - $(element).next().outerWidth(true))) {
            if ((visibleWidth - $(element).next().outerWidth(true)) > marginRightVal) {
                scrollVal = marginLeftVal;
                var tabElement = element;
                while ((scrollVal - $(tabElement).outerWidth()) > ($(".page-tabs-content").outerWidth() - visibleWidth)) {
                    scrollVal -= $(tabElement).prev().outerWidth();
                    tabElement = $(tabElement).prev();
                }
            }
        } else if (marginLeftVal > (visibleWidth - $(element).outerWidth(true) - $(element).prev().outerWidth(true))) {
            scrollVal = marginLeftVal - $(element).prev().outerWidth(true);
        }
        $('.page-tabs-content').animate({
            marginLeft: 0 - scrollVal + 'px'
        }, "fast");
    }
    //查看左侧隐藏的选项卡
    function scrollTabLeft() {
        var marginLeftVal = Math.abs(parseInt($('.page-tabs-content').css('margin-left')));
        // 可视区域非tab宽度
        var tabOuterWidth = calSumWidth($(".content-tabs").children().not(".J_menuTabs"));
        //可视区域tab宽度
        var visibleWidth = $(".content-tabs").outerWidth(true) - tabOuterWidth;
        //实际滚动宽度
        var scrollVal = 0;
        if ($(".page-tabs-content").width() < visibleWidth) {
            return false;
        } else {
            var tabElement = $(".J_menuTab:first");
            var offsetVal = 0;
            while ((offsetVal + $(tabElement).outerWidth(true)) <= marginLeftVal) {//找到离当前tab最近的元素
                offsetVal += $(tabElement).outerWidth(true);
                tabElement = $(tabElement).next();
            }
            offsetVal = 0;
            if (calSumWidth($(tabElement).prevAll()) > visibleWidth) {
                while ((offsetVal + $(tabElement).outerWidth(true)) < (visibleWidth) && tabElement.length > 0) {
                    offsetVal += $(tabElement).outerWidth(true);
                    tabElement = $(tabElement).prev();
                }
                scrollVal = calSumWidth($(tabElement).prevAll());
            }
        }
        $('.page-tabs-content').animate({
            marginLeft: 0 - scrollVal + 'px'
        }, "fast");
    }
    //查看右侧隐藏的选项卡
    function scrollTabRight() {
        var marginLeftVal = Math.abs(parseInt($('.page-tabs-content').css('margin-left')));
        // 可视区域非tab宽度
        var tabOuterWidth = calSumWidth($(".content-tabs").children().not(".J_menuTabs"));
        //可视区域tab宽度
        var visibleWidth = $(".content-tabs").outerWidth(true) - tabOuterWidth;
        //实际滚动宽度
        var scrollVal = 0;
        if ($(".page-tabs-content").width() < visibleWidth) {
            return false;
        } else {
            var tabElement = $(".J_menuTab:first");
            var offsetVal = 0;
            while ((offsetVal + $(tabElement).outerWidth(true)) <= marginLeftVal) {//找到离当前tab最近的元素
                offsetVal += $(tabElement).outerWidth(true);
                tabElement = $(tabElement).next();
            }
            offsetVal = 0;
            while ((offsetVal + $(tabElement).outerWidth(true)) < (visibleWidth) && tabElement.length > 0) {
                offsetVal += $(tabElement).outerWidth(true);
                tabElement = $(tabElement).next();
            }
            scrollVal = calSumWidth($(tabElement).prevAll());
            if (scrollVal > 0) {
                $('.page-tabs-content').animate({
                    marginLeft: 0 - scrollVal + 'px'
                }, "fast");
            }
        }
    }

    //通过遍历给菜单项加上data-index属性
    $(".J_menuItem").each(function (index) {
        if (!$(this).attr('data-index')) {
            $(this).attr('data-index', index);
        }
    });

    function menuItem() {
        // 获取标识数据
        var dataUrl = $(this).attr('href'), dataIndex = $(this).data('index'), menuName = $.trim($(this).text()), flag = true;
        if (dataUrl == undefined || $.trim(dataUrl).length == 0) return false;
        // 选项卡菜单已存在
        $('.J_menuTab').each(function () {
            if ($(this).data('id') == dataUrl) {
                if (!$(this).hasClass('active')) {
                    $(this).addClass('active').siblings('.J_menuTab').removeClass('active');
                    scrollToTab(this);
                    // 显示tab对应的内容区
                    $('.J_mainContent .J_iframe').each(function () {
                        if ($(this).data('id') == dataUrl) {
                            $(this).show().siblings('.J_iframe').hide();
                            return false;
                        }
                    });
                }
                flag = false;
                return false;
            }
        });
        // 选项卡菜单不存在
        if (flag) {
            var str = '<a href="javascript:;" class="active J_menuTab" data-id="' + dataUrl + '">' + menuName + ' <i class="fa fa-times-circle"></i></a>';
            $('.J_menuTab').removeClass('active');
            var height = $(window).height() - 110;
            // 添加选项卡对应的iframe
            var str1 = '<iframe class="J_iframe" name="iframe' + dataIndex + '" width="100%" height="' + height + '" src="' + dataUrl + '" frameborder="0" data-id="' + dataUrl + '" seamless></iframe>';
            $('.J_mainContent').find('iframe.J_iframe').hide().parents('.J_mainContent').append(str1);
            // 添加选项卡
            $('.J_menuTabs .page-tabs-content').append(str);
            scrollToTab($('.J_menuTab.active'));
        }
        return false;
    }
    $('.J_menuItem').on('click', menuItem);
    // 关闭选项卡菜单
    function closeTab() {
        var closeTabId = $(this).parents('.J_menuTab').data('id');
        var currentWidth = $(this).parents('.J_menuTab').width();
        // 当前元素处于活动状态
        if ($(this).parents('.J_menuTab').hasClass('active')) {
            // 当前元素后面有同辈元素，使后面的一个元素处于活动状态
            if ($(this).parents('.J_menuTab').next('.J_menuTab').size()) {
                var activeId = $(this).parents('.J_menuTab').next('.J_menuTab:eq(0)').data('id');
                $(this).parents('.J_menuTab').next('.J_menuTab:eq(0)').addClass('active');
                $('.J_mainContent .J_iframe').each(function () {
                    if ($(this).data('id') == activeId) {
                        $(this).show().siblings('.J_iframe').hide();
                        return false;
                    }
                });
                var marginLeftVal = parseInt($('.page-tabs-content').css('margin-left'));
                if (marginLeftVal < 0) {
                    $('.page-tabs-content').animate({
                        marginLeft: (marginLeftVal + currentWidth) + 'px'
                    }, "fast");
                }
                //  移除当前选项卡
                $(this).parents('.J_menuTab').remove();

                // 移除tab对应的内容区
                $('.J_mainContent .J_iframe').each(function () {
                    if ($(this).data('id') == closeTabId) {
                        $(this).remove();
                        return false;
                    }
                });
            }
            // 当前元素后面没有同辈元素，使当前元素的上一个元素处于活动状态
            if ($(this).parents('.J_menuTab').prev('.J_menuTab').size()) {
                var activeId = $(this).parents('.J_menuTab').prev('.J_menuTab:last').data('id');
                $(this).parents('.J_menuTab').prev('.J_menuTab:last').addClass('active');
                $('.J_mainContent .J_iframe').each(function () {
                    if ($(this).data('id') == activeId) {
                        $(this).show().siblings('.J_iframe').hide();
                        return false;
                    }
                });
                //  移除当前选项卡
                $(this).parents('.J_menuTab').remove();
                // 移除tab对应的内容区
                $('.J_mainContent .J_iframe').each(function () {
                    if ($(this).data('id') == closeTabId) {
                        $(this).remove();
                        return false;
                    }
                });
            }
        }
        // 当前元素不处于活动状态
        else {
            //  移除当前选项卡
            $(this).parents('.J_menuTab').remove();
            // 移除相应tab对应的内容区
            $('.J_mainContent .J_iframe').each(function () {
                if ($(this).data('id') == closeTabId) {
                    $(this).remove();
                    return false;
                }
            });
            scrollToTab($('.J_menuTab.active'));
        }
        return false;
    }
    $('.J_menuTabs').on('click', '.J_menuTab i', closeTab);
    //关闭其他选项卡
    function closeOtherTabs() {
        $('.page-tabs-content').children("[data-id]").not(":first").not(".active").each(function () {
            $('.J_iframe[data-id="' + $(this).data('id') + '"]').remove();
            $(this).remove();
        });
        $('.page-tabs-content').css("margin-left", "0");
    }
    $('.J_tabCloseOther').on('click', closeOtherTabs);
    //滚动到已激活的选项卡
    function showActiveTab() {
        scrollToTab($('.J_menuTab.active'));
    }
    $('.J_tabShowActive').on('click', showActiveTab);
    // 点击选项卡菜单
    function activeTab() {
        if (!$(this).hasClass('active')) {
            var currentId = $(this).data('id');
            // 显示tab对应的内容区
            $('.J_mainContent .J_iframe').each(function () {
                if ($(this).data('id') == currentId) {
                    $(this).show().siblings('.J_iframe').hide();
                    return false;
                }
            });
            $(this).addClass('active').siblings('.J_menuTab').removeClass('active');
            scrollToTab(this);
        }
    }
    $('.J_menuTabs').on('click', '.J_menuTab', activeTab);
    //刷新iframe
    function refreshTab() {
        var target = $('.J_iframe[data-id="' + $(this).data('id') + '"]');
        var url = target.attr('src');
    }
    $('.J_menuTabs').on('dblclick', '.J_menuTab', refreshTab);
    // 左移按扭
    $('.J_tabLeft').on('click', scrollTabLeft);
    // 右移按扭
    $('.J_tabRight').on('click', scrollTabRight);
    // 关闭全部
    $('.J_tabCloseAll').on('click', function () {
        $('.page-tabs-content').children("[data-id]").not(":first").each(function () {
            $('.J_iframe[data-id="' + $(this).data('id') + '"]').remove();
            $(this).remove();
        });
        $('.page-tabs-content').children("[data-id]:first").each(function () {
            $('.J_iframe[data-id="' + $(this).data('id') + '"]').show();
            $(this).addClass("active");
        });
        $('.page-tabs-content').css("margin-left", "0");
    });
    //左侧菜单栏动画
    function e() {
        var e = $("body > #wrapper").height() - 61;
        $(".sidebard-panel").css("min-height", e + "px")
    }
    $("#side-menu").metisMenu(),
        $(".right-sidebar-toggle").click(function () {
            $("#right-sidebar").toggleClass("sidebar-open")
        }),
        $(".sidebar-container").slimScroll({
            height: "100%",
            railOpacity: 0.4,
            wheelStep: 10
        }),
        $(".open-small-chat").click(function () {
            $(this).children().toggleClass("fa-comments").toggleClass("fa-remove"), $(".small-chat-box").toggleClass("active")
        }),
        $(".small-chat-box .content").slimScroll({
            height: "234px",
            railOpacity: 0.4
        }),
        $(".check-link").click(function () {
            var e = $(this).find("i"), a = $(this).next("span");
            return e.toggleClass("fa-check-square").toggleClass("fa-square-o"), a.toggleClass("todo-completed"), !1
        }),
        $(function () {
            $(".sidebar-collapse").slimScroll({
                height: "100%",
                railOpacity: 0.9,
                alwaysVisible: !1
            })
        }),
        $(".navbar-minimalize").click(function () {
            $("body").toggleClass("mini-navbar"), SmoothlyMenu()
        }),
        e(),
        $(window).bind("load resize click scroll", function () {
            $("body").hasClass("body-small") || e()
        }),
        $(window).scroll(function () {
            $(window).scrollTop() > 0 && !$("body").hasClass("fixed-nav") ? $("#right-sidebar").addClass("sidebar-top") : $("#right-sidebar").removeClass("sidebar-top")
        }),
        $(".full-height-scroll").slimScroll({
            height: "100%"
        }),
        $("#side-menu>li").click(function () {
            $("body").hasClass("mini-navbar") && NavToggle()
        }),
        $("#side-menu>li li a").click(function () {
            $(window).width() < 769 && NavToggle()
        }),
        $(".nav-close").click(NavToggle), /(iPhone|iPad|iPod|iOS)/i.test(navigator.userAgent) && $("#content-main").css("overflow-y", "auto"),
        $(window).bind("load resize",
            function () {
                $(this).width() < 769 && ($("body").addClass("mini-navbar"), $(".navbar-static-side").fadeIn())
            }
        ),
        $(function () {
            if ($("#fixednavbar").click(function () {
                $("#fixednavbar").is(":checked") ? ($(".navbar-static-top").removeClass("navbar-static-top").addClass("navbar-fixed-top"), $("body").removeClass("boxed-layout"), $("body").addClass("fixed-nav"), $("#boxedlayout").prop("checked", !1), localStorageSupport && localStorage.setItem("boxedlayout", "off"), localStorageSupport && localStorage.setItem("fixednavbar", "on")) : ($(".navbar-fixed-top").removeClass("navbar-fixed-top").addClass("navbar-static-top"), $("body").removeClass("fixed-nav"), localStorageSupport && localStorage.setItem("fixednavbar", "off"))
            }), $("#collapsemenu").click(function () {
                $("#collapsemenu").is(":checked") ? ($("body").addClass("mini-navbar"), SmoothlyMenu(), localStorageSupport && localStorage.setItem("collapse_menu", "on")) : ($("body").removeClass("mini-navbar"), SmoothlyMenu(), localStorageSupport && localStorage.setItem("collapse_menu", "off"))
            }), $("#boxedlayout").click(function () {
                $("#boxedlayout").is(":checked") ? ($("body").addClass("boxed-layout"), $("#fixednavbar").prop("checked", !1), $(".navbar-fixed-top").removeClass("navbar-fixed-top").addClass("navbar-static-top"), $("body").removeClass("fixed-nav"), localStorageSupport && localStorage.setItem("fixednavbar", "off"), localStorageSupport && localStorage.setItem("boxedlayout", "on")) : ($("body").removeClass("boxed-layout"), localStorageSupport && localStorage.setItem("boxedlayout", "off"))
            }), $(".s-skin-0").click(function () {
                return $("body").removeClass("skin-1"), $("body").removeClass("skin-2"), $("body").removeClass("skin-3"), !1
            }), $(".s-skin-1").click(function () {
                return $("body").removeClass("skin-2"), $("body").removeClass("skin-3"), $("body").addClass("skin-1"), !1
            }), $(".s-skin-3").click(function () {
                return $("body").removeClass("skin-1"), $("body").removeClass("skin-2"), $("body").addClass("skin-3"), !1
            }), localStorageSupport) {
                var e = localStorage.getItem("collapse_menu"), a = localStorage.getItem("fixednavbar"), o = localStorage.getItem("boxedlayout");
                "on" == e && $("#collapsemenu").prop("checked", "checked"), "on" == a && $("#fixednavbar").prop("checked", "checked"), "on" == o && $("#boxedlayout").prop("checked", "checked")
            }
            if (localStorageSupport) {
                var e = localStorage.getItem("collapse_menu"), a = localStorage.getItem("fixednavbar"), o = localStorage.getItem("boxedlayout"), l = $("body");
                "on" == e && (l.hasClass("body-small") || l.addClass("mini-navbar")), "on" == a && ($(".navbar-static-top").removeClass("navbar-static-top").addClass("navbar-fixed-top"), l.addClass("fixed-nav")), "on" == o && l.addClass("boxed-layout")
            }
        });
}