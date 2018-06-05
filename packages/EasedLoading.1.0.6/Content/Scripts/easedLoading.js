(function ($) {

    $("<style type='text/css'>.ball{position: absolute;width:10px;height:10px;background: #5c5c5c;border-radius: 10px;}</style>").appendTo('head');

    $("<style type='text/css'>.loadingText{font-family: Tahoma;color: #5c5c5c;font-size:smaller;display:block;}</style>").appendTo('head');

    $.fn.loading = function (options) {
        var div = this;
		var ballsInView =0;
		var beQuiet = false;
        //clear any loaders:
        var duration = Number(div.attr("fadeOut"));
        var textClass = div.attr("textClass");
        clearInterval(div.attr("intervalID"));
        div.find("." + textClass).animate({
            opacity: 0
        }, duration, function () {
            $(this).remove()
        });
        //start new loaders
        if (options == false) return;
        var args = $.extend({}, {
            animationDuration: 4000,
            newBallsInterval: 400,
            easing: "easeInOutQuart",
            ballCSSClass: "ball",
            textCSSClass: "loadingText",
            text: "",
			groupSize: 0
        }, options);
        if (args.text != "") {
            var t = document.createElement("span");
            $(t).addClass(args.textCSSClass).text(args.text);
            div.append(t);
        }
        var intervalID = setInterval(function () {
	
			if(beQuiet) return;
			
			ballsInView++;
			if(ballsInView>args.groupSize && args.groupSize!=0){
				beQuiet=true;
			}		
			
            var ball = document.createElement("span");
            ball.style.opacity = 0;
			$(ball).offset({left:$(ball).width()});
            $(ball).addClass(args.ballCSSClass);
            $(div).append(ball);
            $(ball).animate({
                opacity: 1
            }, {
                queue: true,
                duration: Number(args.animationDuration) / 2
            });
            $(ball).animate({
                opacity: 0
            }, {
                queue: true,
                duration: Number(args.animationDuration) / 2
            });
            $(ball).animate({
                left: Number(div.width())-Number($(ball).width())				
            }, {
                duration: Number(args.animationDuration),
                easing: args.easing,
                queue: false,
                always: function () {
					ballsInView--;
					if(ballsInView==0)
						beQuiet=false;
                    $(ball).remove(); //ball removed
                }
            });
        }, args.newBallsInterval);
        $(this).attr("intervalID", intervalID).attr("fadeOut", args.animationDuration).attr("textClass", args.textCSSClass);
        return this;
    };

})(jQuery);