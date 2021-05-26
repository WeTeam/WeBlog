<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="/configuration/sitecore/contentSearch/indexConfigurations/defaultWeBlogSolrIndexConfiguration/fieldReaders">
    <fieldReaders type="Sitecore.ContentSearch.FieldReaders.FieldReaderMap, Sitecore.ContentSearch">
      <param desc="id">defaultWeBlogFieldReaderMap</param>
      <mapFieldByTypeName hint="raw:AddFieldReaderByFieldTypeName">
        <fieldReader fieldTypeName="weblog tags" fieldReaderType="Sitecore.Modules.WeBlog.Search.FieldReaders.CsvFieldReader, Sitecore.Modules.WeBlog" />
        <fieldReader fieldTypeName="date|datetime"                                        fieldReaderType="Sitecore.ContentSearch.FieldReaders.DateFieldReader, Sitecore.ContentSearch" />
        <fieldReader fieldTypeName="single-line text|multi-line text|text|memo"           fieldReaderType="Sitecore.ContentSearch.FieldReaders.DefaultFieldReader, Sitecore.ContentSearch" />
        <fieldReader fieldTypeName="html|rich text"                                       fieldReaderType="Sitecore.ContentSearch.FieldReaders.RichTextFieldReader, Sitecore.ContentSearch" />
        <fieldReader fieldTypeName="multilist with search|treelist with search"           fieldReaderType="Sitecore.ContentSearch.FieldReaders.DelimitedListFieldReader, Sitecore.ContentSearch" />
        <fieldReader fieldTypeName="checklist|multilist|treelist|treelistex|tree list"    fieldReaderType="Sitecore.ContentSearch.FieldReaders.MultiListFieldReader, Sitecore.ContentSearch" />
        <fieldReader fieldTypeName="droplink|droptree|grouped droplink|tree|reference"    fieldReaderType="Sitecore.ContentSearch.FieldReaders.LookupFieldReader, Sitecore.ContentSearch" />
      </mapFieldByTypeName>
    </fieldReaders>
  </xsl:template>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>
