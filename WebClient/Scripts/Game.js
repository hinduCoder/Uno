angular.module('Game', [])
    .factory('$game', function () {
        var game = $.connection.gameHub;
        return {
            setHandler: function(name, fn) {
                game.client[name] = fn;
            },
            init: function(callback) {
                $.connection.hub.start();
                game.client.init = callback;
            },
            move: function(index) {
                game.server.move(index);
            },
            draw: game.server.draw,
            pass: game.server.pass,
            uno: game.server.uno,
            chooseColor: game.server.chooseColor 
        }
    });
angular.module('Cards', ['ngSanitize'])
    .directive('card', function() {
        return {
            restrict: 'E',
            template: $('#card-template').html(),
            scope: {
                type: '@',
                number: '@content',
                color: '@'
            },
            link: function(scope, element, attrs, controller) {
                scope.color = scope.color || 'black';

                function setContent() {
                    if (scope.type)
                        scope.content = $('#' + scope.type + '-template').html();
                    else
                        scope.content = scope.number;
                    scope.smallContent = null;
                    switch (scope.type) {
                    case 'plus-two':
                        scope.smallContent = '+2';
                        break;
                    case 'plus-four':
                        scope.smallContent = '+4';
                        break;
                    }
                    scope.smallContent = scope.smallContent || scope.content;
                }

                
                scope.$watch('number', setContent);
                scope.$watch('type', setContent);
                setContent();
            }
        }
    })
    ;
angular.module('App', ['Game', 'ui.bootstrap', 'ngAnimate', 'Cards'])
    .controller('MainController', function($scope, $game, $modal, $alert) {
        var setHandler = function(name, fn) {
            $game.setHandler(name, function() {
                var args = arguments;
                $scope.$apply(function(scope) {
                    fn.apply(scope, args);
                });
            });
        };
        var findOtherPlayerByName = function (name) {
            return $scope.otherPlayers.filter(function (p) { return p.name === name })[0];
        }
        $scope.init = function() {
            $game.init(function(data) {
                $scope.$apply(function(scope) {
                    scope.cards = data.cards;
                    scope.topCard = data.topCard;
                    scope.otherPlayers = data.otherPlayers;
                    scope.deck = data.deck;
                });
            });
            setHandler('activate', function() {
                this.isCurrentPlayer = true;
                this.canDraw = true;
                
            });
            setHandler('discard', function(index, currentPlayer) {
                this.topCard = this.cards.splice(index, 1)[0];
                this.isCurrentPlayer = false;
                findOtherPlayerByName(currentPlayer).active = true;
            });
            setHandler('move', function(top, player, currentPlayer) {
                if (top)
                    this.topCard = top;
                this.canDraw = true;
                this.canPass = false;;
                var prevPlayer = findOtherPlayerByName(player);
                if (prevPlayer) 
                    prevPlayer.cardsCount--;
                this.otherPlayers.forEach(function(p) { p.active = false; });
                var current = findOtherPlayerByName(currentPlayer);
                if (current)
                    current.active = true;
            });
            setHandler('preLastDiscarded', function() {
                $scope.canUno = true;
            });
            setHandler('addCards', function(cards) {
                this.cards = this.cards.concat(cards);
            });
            setHandler('chooseColor', function() {
                var scope = this;
                $modal.open({
                    animation: true,
                    templateUrl: 'chooseColorModalTemplate',
                    controller: 'ChooseColorController'
                }).result.then(function(color) {
                    $game.chooseColor(color);
                    scope.topCard.color = color;
                });
            });
            setHandler('chosenColor', function(color) {
                this.topCard.color = color;
            });
            setHandler('finish', function(scores) {
                $modal.open({
                    templateUrl: 'scoreModalTemplate',
                    controller: 'ScoreController',
                    resolve: { scores: function() { return scores; } }
                });
            });
            setHandler('wrongCard', function() {
                $alert.danger('WRONG CARD!!!');
            });
            setHandler('cardsAdded', function(player, count) {
                var p = this.otherPlayers.filter(function(p) { return p.name === player })[0];
                if (p)
                    p.cardsCount += count;
            });
        }
        $scope.move = function (index) {
            if (!$scope.isCurrentPlayer)
                return;
            $game.move(index);
            $scope.canPass = false;
        };
        $scope.draw = function() {
            $game.draw();
            $scope.canDraw = false;
            $scope.canPass = true;
        }
        $scope.pass = function() {
            $game.pass();
            $scope.isCurrentPlayer = false;
        }
        $scope.uno = function() {
            $game.uno();
            $scope.canUno = false;
        }
    })
    .controller('ChooseColorController', function($scope, $modalInstance) {
        $scope.colors = ['red', 'yellow', 'green', 'blue'];
        $scope.choose = function(color) {
            $modalInstance.close(color);
        }
    })
    .controller('ScoreController', function($scope, scores) {
        $scope.rows = scores;
    })
    .factory('$alert', function() {
        var showAlert = function (type, text, timeout) {
            timeout = timeout || 1000;
            angular.element($('#alerts')).scope().alerts.push({ type: type, text: text, timeout: timeout });
        }
        return {
            success: function(text, timeout) {
                showAlert('success', text, timeout);
            },
            danger: function(text, timeout) {
                showAlert('danger', text, timeout);
            }
        }
    })
    .controller('AlertsController', function($scope) {
        $scope.alerts = [];
        $scope.closeAlert = function(index) {
            $scope.alerts.splice(index, 1);
        }
    });

//var game = $.connection.gameHub;
//$.extend(game.client, {
//    move: function(top) {
//        if (top != undefined) {
//            setTopCard(top);
//        }
//        $('.btn-draw').disabled(false);
//        $('.btn-pass').disabled(true);
//    },
//    discard: function(index) {
//        var card = $('.card').eq(index);
//        var topCard = $('.top-card');
//        topCard.text(card.text());
//        addColorClass(topCard, card.data('color'));
//        card.remove();
//        $('.card').disabled(true);
//    },
//    activate: function() {
//        $('.card').disabled(false);
//        $('.btn-draw').disabled(false);
//    },
//    chooseColor: function() {
//        $('#choose-color-modal').modal('show');
//    },
//    chosenColor: function(color) {
//        addColorClass($('.top-card'), color);
//    },
//    preLastDiscarded: function() {
//        $('.btn-uno').disabled(false);
//    },
//    addCards: function(cards) {
//        cards.forEach(function (card) { addNewCard(card)});
//    },
//    wrongCard: function() {
//        $('#wrong-card-alert').addClass('in');
//        setTimeout(function () { $('#wrong-card-alert').removeClass('in')}, 1000);
//    },
//    finish: function (scores) {
//        $('.card').remove();
//        $('#score-modal .modal-body').html(Mustache.render($('#score-table-template').html(), { table: scores }));
//        $('#score-modal').modal('show');      
//    },
//    win: function() {
//        $('#congrats-alert').addClass('in');
//    },
//    log: function(desc) {
//        $('.list-group').prepend(Mustache.render($('#log-item-template').html(), { description: desc }));
//    },
//    newGame: function(top) {
//        setTopCard(top);
//    }
//});
//function addNewCard(card) {
//    var newButton = $(Mustache.render($('#new-card-template').html(), card));
//    $('.cards').append(newButton);
//    newButton.click(function () {
//        move($(this));
//    });
//    newButton.disabled($('.card').eq(0).disabled());
//}
//$.connection.hub.start().done(function() {
//    $('.card').click(function () {
//        move($(this));
//    });
//    $('.btn-draw').click(function () {
//        draw();
//    });
//    $('.btn-pass').click(function () {
//        pass();
//    });
//    $('.btn-uno').click(function() {
//        game.server.uno();
//        $(this).disabled(true);
//    });
//    $('.modal .btn').click(function() {
//        var color = $(this).data('color');
//        game.server.chooseColor(color);
//        addColorClass($('.top-card'), color);
//    });
//});
//function move(card) {
//    game.server.move(card.index());
//    $('.btn-draw').disabled(true);
//}
//function draw() {
//    game.server.draw();
//    $('.btn-draw').disabled(true);
//    $('.btn-pass').disabled(false);
//    $('.btn-uno').disabled(false);
//}
//function pass() {
//    $('.cards').children().disabled(true);
//    game.server.pass();
//}
//function setTopCard(card) {
//    var topCard = $('.top-card');
//    addColorClass(topCard, card.color);
//    topCard.text(card.content);
//}
//function addColorClass(jq, className) {
//    console.log(className);
//    jq.removeClass('red');
//    jq.removeClass('yellow');
//    jq.removeClass('green');
//    jq.removeClass('blue');
//    jq.removeClass('black');
//    jq.addClass(className);
//    jq.data('color', className);
//}
//$.fn.disabled = function (value) {
//    if (value !== undefined) {
//        $(this).prop('disabled', value);
//        return this;
//    } else
//        return $(this).prop('disabled');
//};