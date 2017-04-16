/*
    Author: Yoadad Santibañez
*/
; (function ($) {
    $.fn.reporting = function (options) {

        var self = this;
        var self = $.extend(self, {
            data: {},
            height: 300,
            width:500,
            sections: {},
            header: null,
            footer: null,
            pageSize: 'letter',
            orientation: 'portrait',
        }, options);

        $(self).css({ width: self.width+'px !important',height:self.height+'px' });

        self.minHeightMargin = 0;//
        self.columnIndex = 0;
        self.footerHeight = 0;
        self.setMaxHeight = function () {
            self.maxHeight = self.orientation == 'portrait' ? 11 * $('.unit').width() : 8.5 * $('.unit').width();
        };

        self.setLandscape = function () {
            $('.reporting-page')
			.removeClass('size-letter')
			.addClass('size-letter-landscape');
        };
        self.setPortrait = function () {
            $('.reporting-page')
			.removeClass('size-letter-landscape')
			.addClass('size-letter');
        };
        self.addPageToPdf = function (root, index) {
            kendo.drawing.drawDOM(".page-" + (index + 1)).then(function (group) {
                group.options.set("pdf", {
                    landscape: self.orientation == 'landscape'
                });
                root.append(group);
                if ($('.reporting-page').size() > (index + 1)) {
                    self.addPageToPdf(root, index + 1);
                }
                else {
                    if ($('.reporting-page').size() > 1) {
                        root.options.set("pdf", {
                            multiPage: 'true'
                        });
                    }
                    kendo.drawing.pdf.saveAs(root, "Transcript.pdf", self.urlProxy);
                }
            });

        };

        self.exportPDF = function () {
            root = new kendo.drawing.Group();
            var index = 0;
            self.addPageToPdf(root, index);
        };

        if (typeof options == 'string') {
            switch (options.toLowerCase()) {
                case 'export-pdf':
                    self.exportPDF();
                    break;
                case 'landscape':
                    self.setLandscape();
                    break;
                case 'portrait':
                    self.setPortrait();
                    break;
            }
            return self;
        }

        self.setFooterHeight = function () {
            if (self.footer) {
                self.drawSection(self.footer);
                self.footerHeight = self.find('.reporting-section:last').height() || 0;
                self.find('.reporting-section:last').remove();
                self.minHeightMargin = self.footerHeight + 21;
            }
        };

        self.addPage = function () {
            self.append(self.getHtmlNewPage());
            self.currentPage = self.find('.reporting-page:last');
            var pageNumber = self.find('.reporting-page').size();
            if (pageNumber > 1) {
                self.previousPage = self.find('.reporting-page:eq(' + (pageNumber - 2) + ')');
            }
            self.drawSection(self.header);
        };

        self.getHtmlNewPage = function () {
            var index = self.find('.reporting-page').size() + 1;
            var sizeClass = self.orientation == 'portrait' ? 'size-letter' : 'size-letter-landscape';
            return '<br/><div class="reporting-page ' + sizeClass + ' page-' + index + '"></div>';
        };
        self.getHtmlTemplate = function (html, data) {
            var template = kendo.template(html);
            var result = template(data);
            return result;
        };
        self.getColumnSize = function (count) {
            return 12 / count;
        }
        self.getHtmlColumn = function (content, count) {
            var result = '<div class="col-sm-' + self.getColumnSize(count) + ' reporting-content">';
            result += self.getHtmlTemplate(content.template, self.data);
            result += '</div>';
            return result;
        };
        self.getHtmlDetailHeader = function (detail) {
            var result = '<thead>';
            result += '<tr>';
            for (var i = 0; i < detail.fields.length; i++) {
                if (detail.fields[i].visible != false) {
                    result += self.getHtmlTemplate(detail.fields[i].titleTemplate, detail.fields[i]);
                }
            }
            result += '</tr>';
            result += '</thead>';
            return result;
        };
        self.isGroupingSeparator = function (detail, index1, index2) {
            return self.getConcatData(detail, index1) != self.getConcatData(detail, index2);

        }
        self.getHtmlDetailBody = function (detail) {
            var result = '<tbody>';
            result += '</tbody>';
            return result;
        };

        self.sortDetailByGroupings = function (detail) {
            if (detail.grouping) {
                self.sortByGroup(detail);
            }
        };

        self.sortByField = function (data, fieldName) {
            var aux;
            for (var i = 0; i < data.length; i++) {
                for (var j = i + 1; j < data.length; j++) {
                    if (data[i][fieldName] > data[j][fieldName]) {
                        aux = data[i];
                        data[i] = data[j];
                        data[j] = aux;
                    }
                }
            }
            return data;
        };
        self.getConcatData = function (detail, idx) {
            var result = '';
            var data = detail.data[idx];
            for (var i = 0; i < detail.grouping.length; i++) {
                result += data[detail.grouping[i]];
            }
            return result;
        };

        self.isData1BiggerData2 = function (detail, index1, index2) {
            return self.getConcatData(detail, index1) > self.getConcatData(detail, index2);
        };

        self.sortByGroup = function (detail) {
            var aux;
            for (var i = 0; i < detail.data.length; i++) {
                for (var j = i + 1; j < detail.data.length; j++) {
                    if (self.isData1BiggerData2(detail, i, j)) {
                        aux = detail.data[i];
                        detail.data[i] = detail.data[j];
                        detail.data[j] = aux;
                    }
                }
            }
            return detail.data;
        };

        self.isGroupingCurrentData = function (data, currentData) {
            var result = detail.grouping.length > 0;
            for (var j = 0; j < detail.grouping.length; j++) {
                if (data[detail.grouping[i]] != currentData[detail.grouping[i]]) {
                    result = false;
                }
            }
            return result;
        };

        self.getDetailGrouping = function (detail) {
            var index = 0;
            var result = [];
            var currentData = detail.data[0];
            result[index] = [];
            for (var j = 0; j < detail.data.length; j++) {
                if (isGroupingCurrentData(detail.data[i], currentData)) {
                    result[index].push(detail.data[i]);
                }
                else {

                }
            }
            return result;
        };

        self.getHtmlDetail = function (detail) {
            var result = '<table class="table table-striped">';
            result += self.getHtmlDetailHeader(detail);
            result += self.getHtmlDetailBody(detail);
            result += '</table>';
            return result || '';
        };

        self.getHtmlColumnDetail = function (detail, count, isEmpty) {
            var columnSize = self.getColumnSize(count);
            var styleRight = detail.columnNumber == 2 ? 'style = "margin-left:0; padding-left: 0px;"' : '';
            var styleLeft = detail.columnNumber == 2 ? 'style = "margin-left:0; padding-right: 0px;"' : '';
            var result = '<div class="col-sm-' + columnSize + ' reporting-detail-column" ' + styleLeft+ '>';
            result += self.getHtmlDetail(detail);
            result += '</div>';
            for (var i = 1; i < (12 / columnSize) ; i++) {
                result += '<div class="col-sm-' + columnSize + ' reporting-detail-column"' + styleRight + '>';
                result += self.getHtmlDetail(detail);
                result += '</div>';
            }
            return result;
        };

        self.getEmptyDetail = function ($detail) {
            var html = $detail[0].outerHTML;
            self.currentPage.prepend(html);
            var $emptyDetail = self.currentPage.find('.reporting-detail:first');
            $emptyDetail.find('table tbody').html('');
            return $emptyDetail;
        };

        self.getHtmlSection = function (section) {
            var result = '<div class="row reporting-section" style="' + (section.style || '') + '">';
            for (var i = 0; i < section.contents.length; i++) {
                result += self.getHtmlColumn(section.contents[i], section.contents.length);
            }
            result += '</div>';
            return result;
        };

        self.getHtmlSectionDetail = function (section) {
            var result = '';
            self.sortDetailByGroupings(section.detail);
            result += '<div class="row reporting-detail" style="' + (section.style || '') + '">';
            result += self.getHtmlColumnDetail(section.detail, section.detail.columnNumber || 1);
            result += '</div>';
            return result;
        };



        self.getCurrentPageSectionsHeight = function () {
            var result = 0.0;
            self.currentPage.find('.reporting-section,.reporting-detail')
            .each(function () {
                result += $(this).height();
            });
            return result;
        };

        self.fixNewPageSectionValidation = function () {
            if (self.getCurrentPageSectionsHeight() > (self.maxHeight - self.minHeightMargin)) {
                self.addPage();
                var section =   self.previousPage
                                .find('.reporting-section:last')
                                .remove()
                                .appendTo(self.currentPage);
            }
        };
        self.fixNewPageDetailValidation = function () {
            if (self.getCurrentPageSectionsHeight() > (self.maxHeight - self.minHeightMargin)) {
                self.addPage();
                var section = self.previousPage
                                .find('.reporting-detail:last')
                                .remove()
                                .appendTo(self.currentPage);
            }
        };


        self.drawSection = function (section,ignoreValidation) {
            if (section) {
                var sectionHtml = self.getHtmlSection(section);
                self.currentPage.append(sectionHtml);
                if (!ignoreValidation) {
                    self.fixNewPageSectionValidation();
                }
            }
        };

        self.drawDetail = function (section) {
            if (section.detail) {
                var detailHtml = self.getHtmlSectionDetail(section);
                self.currentPage.append(detailHtml);
                self.fixNewPageDetailValidation();
                self.drawData(section);
            }
        };

        self.drawData = function (section) {
            var detail = section.detail;
            var currentDataIndex = 0;
            for (var j = 0; j < detail.data.length; j++) {
                var classSeparator = '';
                if (detail.grouping && self.isGroupingSeparator(detail, currentDataIndex, j)) {
                    classSeparator = "reporting-detail-separator";
                    currentDataIndex = j;
                }
                self.drawRow(section,j,classSeparator);
            }
        };

        self.drawRow = function (section, index, classSeparator) {
            var detail = section.detail;
            var rowHtml = self.getRowHtml(detail, index, classSeparator);

            var currentColumnBody;
            var nextColumnBody;

            switch (self.columnIndex) {
                case 0:
                    currentColumnBody = self.currentPage.find('.reporting-detail:last>.reporting-detail-column:first>table>tbody');
                    currentColumnBody.append(rowHtml);
                    if (self.getCurrentPageSectionsHeight() > (self.maxHeight - self.minHeightMargin)) {
                        self.columnIndex = 1;
                        nextColumnBody = self.currentPage.find('.reporting-detail:last>.reporting-detail-column:last>table>tbody');
                        currentColumnBody.find('tr:last')
                                                .remove()
                                                .appendTo(nextColumnBody);
                    }
                    break;
                case 1:
                    currentColumnBody = self.currentPage.find('.reporting-detail:last>.reporting-detail-column:last>table>tbody');
                    currentColumnBody.append(rowHtml);
                    if (self.getCurrentPageSectionsHeight() > (self.maxHeight - self.minHeightMargin)) {
                        self.columnIndex = 0;
                        self.addPage();
                        var detailHtml = self.getHtmlSectionDetail(section);
                        self.currentPage.append(detailHtml);
                        nextColumnBody = self.currentPage.find('.reporting-detail:last>.reporting-detail-column:first>table>tbody');
                        currentColumnBody.find('tr:last')
                                                .remove()
                                                .appendTo(nextColumnBody);
                    }
                    break;
            }
        };

        self.getRowHtml = function (detail,index, classSeparator) {
            var result = '<tr class="reporting-detail-row ' + classSeparator + '">';
            for (var i = 0; i < detail.fields.length; i++) {
                if (detail.fields[i].visible != false) {
                    var template = detail.fields[i].template || '<td>#=data.' + detail.fields[i].name + '#</td>';
                    result += self.getHtmlTemplate(template, detail.data[index]);
                }
            }
            result += '</tr>';
            return result;
        };


        self.drawSections = function () {
            for (var i = 0; i < self.sections.length; i++) {
                self.drawSection(self.sections[i]);
                self.drawDetail(self.sections[i]);
                self.columnIndex = 0;
                self.fixDetailColumns();
            }
        };

        self.fixHeightDetailColumns = function ($detail, count) {
            var $colum1 = $detail.find('.reporting-detail-column:eq(0)');
            var $colum2 = $detail.find('.reporting-detail-column:eq(1)');
            var diff1 = 0, diff2 = 0;
            while ($colum1.height() >= $colum2.height()) {
                $colum1.find('.reporting-detail-row:last').remove().prependTo($colum2.find('tbody'));
            };
            diff1 = Math.abs($colum1.height() - $colum2.height());
            $colum2.find('.reporting-detail-row:first').remove().appendTo($colum1.find('tbody'));
            diff2 = Math.abs($colum1.height() - $colum2.height());
            if (diff2 > diff1) {
                $colum1.find('.reporting-detail-row:last').remove().prependTo($colum2.find('tbody'));
            }
        };

        self.fixDetailColumns = function () {
            var reportDetailSection = self.currentPage.find('.reporting-detail:last');
            var columnNumber = reportDetailSection.find('.reporting-detail-column').size();
            if (columnNumber > 1) {
                self.fixHeightDetailColumns(reportDetailSection, columnNumber);
            }
            
        };
        self.getMaxHeightColumn = function ($detail) {
            var columnNumber = $detail.find('.reporting-detail-column').size();
            if (columnNumber > 1) {
                return $detail.find('.reporting-detail-column:first').height() > $detail.find('.reporting-detail-column:last').height()
				? 0
				: 1;
            }
            else {
                return 0;
            }
        };

        self.drawReport = function ($viewer) {
            $viewer.html('<div class="unit"></div>').addClass('reporting-viewer')
			.addClass('back');
            self.setMaxHeight();
            self.addPage();
            self.setFooterHeight();
            if (self.orientation.toLowerCase() == 'landscape') {
                self.setLandscape();
            };
            self.drawSections();
            self.drawFooters();
        };

        self.GetBlankSectionBeforeLastFooter = function () {
            var height = self.maxHeight - self.getCurrentPageSectionsHeight() - self.minHeightMargin;
            var $lastPage = $('.reporting-page:last');
            $lastPage.find('.reporting-section,.reporting-detail').each(function () { });
            var result = {
                contents: [
                    { template: '<div class="container-fluid page-item"><div class="row"><div class="col-xs-12"><div style="height:'+height+'px;"><div></div></div></div>' }
                ]
            };
            return result;
        };

        self.drawFooters = function () {
            var $pages = $('.reporting-page:not(:last-child)');
            var $lastPage = $('.reporting-page:last');
            self.data.Footer = self.data.Footer || {};
            self.data.Footer.Pages = $pages.size() + 1;
            $pages.each(function () {
                self.currentPage = $(this);
                self.drawSection(self.footer,true);
            });
            self.currentPage = $lastPage;
            var blankSection = self.GetBlankSectionBeforeLastFooter();
            self.drawSection(blankSection, true);
            self.drawSection(self.footer, true);
        };

        $(this).each(function (index) {
            self.drawReport($(this));
        });
        return self;
    };
}(jQuery));



