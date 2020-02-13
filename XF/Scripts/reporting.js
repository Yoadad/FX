/*
    Author: Yoadad Santibañez
*/

var REPORTING = REPORTING || {};
; (function ($, RPT) {
    $.fn.reporting = function (options) {
        var $RPT = $(this);

        RPT = $.extend(RPT, {
            data: {},
            height: 300,
            width: 500,
            sections: {},
            header: null,
            footer: null,
            pageSize: 'letter',
            orientation: 'portrait',
            fileName: 'print.pdf'
        }, options);

        //$RPT.css({ height: RPT.height + 'px' });

        RPT.minHeightMargin = 0;
        RPT.columnIndex = 0;
        RPT.footerHeight = 0;
        RPT.setMaxHeight = function () {
            RPT.maxHeight = RPT.orientation == 'portrait' ? 11 * $('.unit').width() : 8.5 * $('.unit').width();
        };

        RPT.setLandscape = function () {
            $('.reporting-page')
			.removeClass('size-letter')
			.addClass('size-letter-landscape');
        };

        RPT.setPortrait = function () {
            $('.reporting-page')
			.removeClass('size-letter-landscape')
			.addClass('size-letter');
        };

        RPT.addPageToPdf = function (root, n, index, isExport) {

            console.log(index);
            kendo.drawing.drawDOM(".page-" + (index + 1))
            .then(function (group) {
                group.options.set("pdf", {
                    landscape: RPT.orientation == 'landscape'
                });
                root.append(group);
                if (n > (index + 1)) {
                    RPT.addPageToPdf(root, n, index + 1);
                }
                else {

                    if (!isExport) {
                        kendo.drawing.pdf.toDataURL(root, function (dataURL) {
                            kendo.saveAs({
                                dataURI: dataURL,
                                fileName: "print.pdf",
                                proxyURL: RPT.urlProxy,
                                forceProxy: true,
                                proxyTarget: "_blank"
                            });
                        });

                    } else {
                        kendo.drawing.pdf.saveAs(root, RPT.fileName, RPT.urlProxy);
                    }
                }
            });
        };

        RPT.exportPDF = function (isExport) {
            var root = new kendo.drawing.Group();
            var n = $('.reporting-page').size();
            var index = 0;
            if (n > 1) {
                root.options.set("pdf", {
                    multiPage: 'true'
                });
            }
            RPT.addPageToPdf(root, n, index, isExport);
        };

        if (typeof options == 'string') {
            switch (options.toLowerCase()) {
                case 'export-pdf':
                    RPT.exportPDF(true);
                    break;
                case 'open-pdf':
                    RPT.exportPDF(false);
                    break;
                case 'landscape':
                    RPT.setLandscape();
                    break;
                case 'portrait':
                    RPT.setPortrait();
                    break;
            }
            return RPT;
        }

        RPT.setFooterHeight = function () {
            if (RPT.footer) {
                RPT.drawSection(RPT.footer);
                RPT.footerHeight = $RPT.find('.reporting-section:last').height() || 0;
                $RPT.find('.reporting-section:last').remove();
                RPT.minHeightMargin = RPT.footerHeight + 21;
            }
        };

        RPT.addPage = function () {
            $RPT.append(RPT.getHtmlNewPage());
            RPT.currentPage = $RPT.find('.reporting-page:last');
            var pageNumber = $RPT.find('.reporting-page').size();
            if (pageNumber > 1) {
                RPT.previousPage = $RPT.find('.reporting-page:eq(' + (pageNumber - 2) + ')');
            }
            RPT.drawSection(RPT.header);
        };

        RPT.getHtmlNewPage = function () {
            var index = $RPT.find('.reporting-page').size() + 1;
            var sizeClass = RPT.orientation == 'portrait' ? 'size-letter' : 'size-letter-landscape';
            return '<br/><div class="reporting-page ' + sizeClass + ' page-' + index + '"></div>';
        };
        RPT.getHtmlTemplate = function (html, data) {
            var template = kendo.template(html);
            var result = template(data);
            return result;
        };
        RPT.getColumnSize = function (count) {
            return 12 / count;
        }
        RPT.getHtmlColumn = function (content, count) {
            var result = '<div class="col-sm-' + RPT.getColumnSize(count) + ' reporting-content">';
            result += RPT.getHtmlTemplate(content.template, RPT.data);
            result += '</div>';
            return result;
        };
        RPT.getHtmlDetailHeader = function (detail) {
            var result = '<thead>';
            result += '<tr>';
            for (var i = 0; i < detail.fields.length; i++) {
                if (detail.fields[i].visible != false) {
                    result += RPT.getHtmlTemplate(detail.fields[i].titleTemplate, detail.fields[i]);
                }
            }
            result += '</tr>';
            result += '</thead>';
            return result;
        };
        RPT.isGroupingSeparator = function (detail, index1, index2) {
            return RPT.getConcatData(detail, index1) != RPT.getConcatData(detail, index2);

        }
        RPT.getHtmlDetailBody = function (detail) {
            var result = '<tbody>';
            result += '</tbody>';
            return result;
        };
        RPT.getHtmlDetailFooter = function (detail) {
            if (detail.footer) {
                var result = '<tfoot>';
                result += '<tr>';
                for (var i = 0; i < detail.fields.length; i++) {
                    result += '<td class="text-right foot">';
                    for (var j = 0; j < detail.footer.length; j++) {
                        if (detail.fields[i].name == detail.footer[j].name) {
                            var footerResult = RPT.getSumFooter(detail, detail.footer[j].name);
                            var template = detail.footer[j].template;
                            result += RPT.getHtmlTemplate(template, { value: footerResult});
                        }
                    }
                    result += '</td>';
                }
                result += '</tr>';
                result += '</tfoot>';
                return result;
            }
            return '';
        };

        RPT.getSumFooter = function (detail, fieldName) {
            var sum = 0;
            for (var i = 0; i < detail.data.length; i++) {
                sum += detail.data[i][fieldName];
            }
            return sum;
        }

        RPT.sortDetailByGroupings = function (detail) {
            if (detail.grouping) {
                RPT.sortByGroup(detail);
            }
        };

        RPT.sortByField = function (data, fieldName) {
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
        RPT.getConcatData = function (detail, idx) {
            var result = '';
            var data = detail.data[idx];
            for (var i = 0; i < detail.grouping.length; i++) {
                result += data[detail.grouping[i]];
            }
            return result;
        };

        RPT.isData1BiggerData2 = function (detail, index1, index2) {
            return RPT.getConcatData(detail, index1) > RPT.getConcatData(detail, index2);
        };

        RPT.sortByGroup = function (detail) {
            var aux;
            for (var i = 0; i < detail.data.length; i++) {
                for (var j = i + 1; j < detail.data.length; j++) {
                    if (RPT.isData1BiggerData2(detail, i, j)) {
                        aux = detail.data[i];
                        detail.data[i] = detail.data[j];
                        detail.data[j] = aux;
                    }
                }
            }
            return detail.data;
        };

        RPT.isGroupingCurrentData = function (data, currentData, detail) {
            var result = detail.grouping.length > 0;
            for (var i = 0; i < detail.grouping.length; i++) {
                if (data[detail.grouping[i]] != currentData[detail.grouping[i]]) {
                    result = false;
                    break;
                }
            }
            return result;
        };

        RPT.getDetailGrouping = function (detail) {
            var index = 0;
            var result = [];
            var currentData = detail.data[0];
            result[index] = [];
            for (var j = 0; j < detail.data.length; j++) {
                if (RPT.isGroupingCurrentData(detail.data[i], currentData, detail)) {
                    result[index].push(detail.data[i]);
                }
                else {

                }
            }
            return result;
        };

        RPT.getHtmlDetail = function (detail) {
            var result = '<table class="table table-striped">';
            result += RPT.getHtmlDetailHeader(detail);
            result += RPT.getHtmlDetailBody(detail);
            result += '</table>';
            return result || '';
        };

        RPT.getHtmlColumnDetail = function (detail, count, isEmpty) {
            var columnSize = RPT.getColumnSize(count);
            var styleRight = detail.columnNumber == 2 ? 'style = "margin-left:0; padding-left: 0px;"' : '';
            var styleLeft = detail.columnNumber == 2 ? 'style = "margin-left:0; padding-right: 0px;"' : '';
            var result = '<div class="col-sm-' + columnSize + ' reporting-detail-column" ' + styleLeft + '>';
            result += RPT.getHtmlDetail(detail);
            result += '</div>';
            for (var i = 1; i < (12 / columnSize) ; i++) {
                result += '<div class="col-sm-' + columnSize + ' reporting-detail-column"' + styleRight + '>';
                result += RPT.getHtmlDetail(detail);
                result += '</div>';
            }
            return result;
        };

        RPT.getEmptyDetail = function ($detail) {
            var html = $detail[0].outerHTML;
            RPT.currentPage.prepend(html);
            var $emptyDetail = RPT.currentPage.find('.reporting-detail:first');
            $emptyDetail.find('table tbody').html('');
            return $emptyDetail;
        };

        RPT.getHtmlSection = function (section, aditionalClass) {
            aditionalClass = ' ' + aditionalClass || '';
            var result = '<div class="row reporting-section' + aditionalClass + '" style="' + (section.style || '') + '">';
            for (var i = 0; i < section.contents.length; i++) {
                result += RPT.getHtmlColumn(section.contents[i], section.contents.length);
            }
            result += '</div>';
            return result;
        };

        RPT.getHtmlSectionDetail = function (section) {
            var result = '';
            RPT.sortDetailByGroupings(section.detail);
            result += '<div class="row reporting-detail" style="' + (section.style || '') + '">';
            result += RPT.getHtmlColumnDetail(section.detail, section.detail.columnNumber || 1);
            result += '</div>';
            return result;
        };



        RPT.getCurrentPageSectionsHeight = function () {
            var result = 0.0;
            RPT.currentPage.find('.reporting-section,.reporting-detail')
            .each(function () {
                result += $(this).height();
            });
            return result;
        };

        RPT.fixNewPageSectionValidation = function () {
            if (RPT.getCurrentPageSectionsHeight() > (RPT.maxHeight - RPT.minHeightMargin)) {
                RPT.addPage();
                var section = RPT.previousPage
                                .find('.reporting-section:last')
                                .remove()
                                .appendTo(RPT.currentPage);
            }
        };
        RPT.fixNewPageDetailValidation = function () {
            if (RPT.getCurrentPageSectionsHeight() > (RPT.maxHeight - RPT.minHeightMargin)) {
                RPT.addPage();
                var section = RPT.previousPage
                                .find('.reporting-detail:last')
                                .remove()
                                .appendTo(RPT.currentPage);
            }
        };


        RPT.drawSection = function (section, ignoreValidation, aditionalClass) {
            aditionalClass = ' ' + aditionalClass || '';
            if (section) {
                var sectionHtml = RPT.getHtmlSection(section, aditionalClass);
                RPT.currentPage.append(sectionHtml);
                if (!ignoreValidation) {
                    RPT.fixNewPageSectionValidation();
                }
            }
        };

        RPT.drawDetail = function (section) {
            if (section.detail) {
                var detailHtml = RPT.getHtmlSectionDetail(section);
                RPT.currentPage.append(detailHtml);
                RPT.fixNewPageDetailValidation();
                RPT.drawData(section);
            }
        };

        RPT.drawData = function (section) {
            var detail = section.detail;
            var currentDataIndex = 0;
            for (var j = 0; j < detail.data.length; j++) {
                var classSeparator = '';
                if (detail.grouping && RPT.isGroupingSeparator(detail, currentDataIndex, j)) {
                    classSeparator = "reporting-detail-separator";
                    currentDataIndex = j;
                }
                RPT.drawRow(section, j, classSeparator);
            }
        };

        RPT.drawRow = function (section, index, classSeparator) {
            var detail = section.detail;
            var rowHtml = RPT.getRowHtml(detail, index, classSeparator);

            var currentColumnBody;
            var nextColumnBody;
            var columnsNumber = RPT.currentPage.find('.reporting-detail:last>.reporting-detail-column').size();

            if (columnsNumber == 2) {
                switch (RPT.columnIndex) {
                    case 0:
                        currentColumnBody = RPT.currentPage.find('.reporting-detail:last>.reporting-detail-column:first>table>tbody');
                        currentColumnBody.append(rowHtml);
                        if (RPT.getCurrentPageSectionsHeight() > (RPT.maxHeight - RPT.minHeightMargin)) {
                            RPT.columnIndex = 1;
                            nextColumnBody = RPT.currentPage.find('.reporting-detail:last>.reporting-detail-column:last>table>tbody');
                            currentColumnBody.find('tr:last')
                                                    .remove()
                                                    .appendTo(nextColumnBody);
                        }
                        break;
                    case 1:
                        currentColumnBody = RPT.currentPage.find('.reporting-detail:last>.reporting-detail-column:last>table>tbody');
                        currentColumnBody.append(rowHtml);
                        if (RPT.getCurrentPageSectionsHeight() > (RPT.maxHeight - RPT.minHeightMargin)) {
                            RPT.columnIndex = 0;
                            RPT.addPage();
                            var detailHtml = RPT.getHtmlSectionDetail(section);
                            RPT.currentPage.append(detailHtml);
                            nextColumnBody = RPT.currentPage.find('.reporting-detail:last>.reporting-detail-column:first>table>tbody');
                            currentColumnBody.find('tr:last')
                                                    .remove()
                                                    .appendTo(nextColumnBody);
                        }
                        break;
                }

            }
            else {

                currentColumnBody = RPT.currentPage.find('.reporting-detail:last>.reporting-detail-column:last>table>tbody');
                currentColumnBody.append(rowHtml);
                if (RPT.getCurrentPageSectionsHeight() > (RPT.maxHeight - RPT.minHeightMargin)) {
                    RPT.columnIndex = 0;
                    RPT.addPage();
                    var detailHtml = RPT.getHtmlSectionDetail(section);
                    RPT.currentPage.append(detailHtml);
                    nextColumnBody = RPT.currentPage.find('.reporting-detail:last>.reporting-detail-column:first>table>tbody');
                    currentColumnBody.find('tr:last')
                                            .remove()
                                            .appendTo(nextColumnBody);

                }

            }



        };

        RPT.getRowHtml = function (detail, index, classSeparator) {
            var result = '<tr class="reporting-detail-row ' + classSeparator + '">';
            for (var i = 0; i < detail.fields.length; i++) {
                if (detail.fields[i].visible != false) {
                    var template = detail.fields[i].template || '<td>#=data.' + detail.fields[i].name + '#</td>';
                    result += RPT.getHtmlTemplate(template, detail.data[index]);
                }
            }
            result += '</tr>';
            return result;
        };


        RPT.drawSections = function () {
            for (var i = 0; i < RPT.sections.length; i++) {
                RPT.drawSection(RPT.sections[i]);
                RPT.drawDetail(RPT.sections[i]);
                RPT.columnIndex = 0;
                RPT.fixDetailColumns();
                RPT.setLastDetailFooter(RPT.sections[i]);
            }
        };

        RPT.setLastDetailFooter = function (section) {
            var detailFooterHtml = RPT.getHtmlDetailFooter(section.detail);
            $('.reporting-detail-column table:last').append(detailFooterHtml);
        };

        RPT.fixHeightDetailColumns = function ($detail, count) {
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

        RPT.fixDetailColumns = function () {
            var reportDetailSection = RPT.currentPage.find('.reporting-detail:last');
            var columnNumber = reportDetailSection.find('.reporting-detail-column').size();
            if (columnNumber > 1) {
                RPT.fixHeightDetailColumns(reportDetailSection, columnNumber);
            }

        };
        RPT.getMaxHeightColumn = function ($detail) {
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

        RPT.drawReport = function ($viewer) {
            $viewer.html('<div class="unit"></div>').addClass('reporting-viewer')
            .addClass('back');
            RPT.setMaxHeight();
            RPT.addPage();
            RPT.setFooterHeight();
            if (RPT.orientation.toLowerCase() == 'landscape') {
                RPT.setLandscape();
            };
            console.log(':)');
            RPT.drawSections();
            RPT.drawFooters();
        };

        RPT.GetBlankSectionBeforeLastFooter = function () {
            var height = RPT.maxHeight - RPT.getCurrentPageSectionsHeight() - RPT.minHeightMargin;
            var $lastPage = $('.reporting-page:last');
            $lastPage.find('.reporting-section,.reporting-detail').each(function () { });
            var result = {
                contents: [
                    { template: '<div class="container-fluid page-item"><div class="row"><div class="col-xs-12"><div style="height:' + height + 'px;"><div></div></div></div>' }
                ]
            };
            return result;
        };

        RPT.drawFooters = function () {
            var $pages = $('.reporting-page:not(:last-child)');
            var $lastPage = $('.reporting-page:last');
            RPT.data.Footer = RPT.data.Footer || {};
            RPT.data.Footer.Pages = $pages.size() + 1;
            $RPT.find('.reporting-footer').remove();
            $pages.each(function () {
                RPT.currentPage = $(this);
                RPT.drawSection(RPT.footer, true, 'reporting-footer');
            });
            RPT.currentPage = $lastPage;
            var blankSection = RPT.GetBlankSectionBeforeLastFooter();
            RPT.drawSection(blankSection, true, 'reporting-blank');
            RPT.drawSection(RPT.footer, true, 'reporting-footer');
        };
        RPT.addPages = function (options) {
            $.extend(RPT, options || {});
            RPT.addPage();
            RPT.setFooterHeight();
            if (RPT.orientation.toLowerCase() == 'landscape') {
                RPT.setLandscape();
            };
            RPT.drawSections();
            RPT.drawFooters();
        };

        $RPT.each(function (index) {
            RPT.drawReport($(this));
        });
        return RPT;
    };
}(jQuery, REPORTING));



